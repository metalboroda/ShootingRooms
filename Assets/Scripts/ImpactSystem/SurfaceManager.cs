using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ImpactSystem;
using Lean.Pool;
using UnityEngine;

namespace ImpactSystem
{
    public class SurfaceManager : MonoBehaviour
    {
        public static SurfaceManager Instance { get; private set; }

        [SerializeField] private SurfaceSO defaultSurface;
        [SerializeField] private List<SurfaceType> surfaces = new List<SurfaceType>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void HandleImpact(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal, ImpactTypeSO impactType,
            int triangleIndex)
        {
            if (hitObject.TryGetComponent(out Terrain terrain))
            {
                List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, hitPoint);

                foreach (TextureAlpha activeTexture in activeTextures)
                {
                    SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture.Texture);

                    if (surfaceType != null)
                    {
                        foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                        {
                            if (typeEffect.ImpactType == impactType)
                            {
                                PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, activeTexture.Alpha);
                            }
                        }
                    }
                    else
                    {
                        foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in defaultSurface.ImpactTypeEffects)
                        {
                            if (typeEffect.ImpactType == impactType)
                            {
                                PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                            }
                        }
                    }
                }
            }
            else if (hitObject.TryGetComponent(out Renderer rend))
            {
                Texture activeTexture = GetActiveTextureFromRenderer(rend, triangleIndex);
                SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture);

                if (surfaceType != null)
                {
                    foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == impactType)
                        {
                            PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                        }
                    }
                }
                else
                {
                    if (defaultSurface == null)
                    {
                        return;
                    }

                    foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in defaultSurface.ImpactTypeEffects)
                    {
                        if (typeEffect.ImpactType == impactType)
                        {
                            PlayEffects(hitPoint, hitNormal, typeEffect.SurfaceEffect, 1);
                        }
                    }
                }
            }
        }

        private List<TextureAlpha> GetActiveTexturesFromTerrain(Terrain terrain, Vector3 hitPoint)
        {
            Vector3 terrainPosition = hitPoint - terrain.transform.position;
            Vector3 splatMapPosition = new Vector3(
                terrainPosition.x / terrain.terrainData.size.x,
                0,
                terrainPosition.z / terrain.terrainData.size.z
            );

            int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
            int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

            List<TextureAlpha> activeTextures = new List<TextureAlpha>();

            for (int i = 0; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > 0)
                {
                    activeTextures.Add(new TextureAlpha()
                    {
                        Texture = terrain.terrainData.terrainLayers[i].diffuseTexture,
                        Alpha = alphaMap[0, 0, i]
                    });
                }
            }

            return activeTextures;
        }

        private Texture GetActiveTextureFromRenderer(Renderer rend, int triangleIndex)
        {
            if (rend.TryGetComponent(out MeshFilter meshFilter))
            {
                Mesh mesh = meshFilter.mesh;

                if (mesh.subMeshCount > 1)
                {
                    int[] hitTriangleIndices = new int[]
                    {
                        mesh.triangles[triangleIndex * 3],
                        mesh.triangles[triangleIndex * 3 + 1],
                        mesh.triangles[triangleIndex * 3 + 2]
                    };

                    for (int i = 0; i < mesh.subMeshCount; i++)
                    {
                        int[] submeshTriangles = mesh.GetTriangles(i);

                        for (int j = 0; j < submeshTriangles.Length; j += 3)
                        {
                            if (submeshTriangles[j] == hitTriangleIndices[0]
                                && submeshTriangles[j + 1] == hitTriangleIndices[1]
                                && submeshTriangles[j + 2] == hitTriangleIndices[2])
                            {
                                return rend.sharedMaterials[i].mainTexture;
                            }
                        }
                    }
                }
                else
                {
                    return rend.sharedMaterial.mainTexture;
                }
            }

            return null;
        }

        private void PlayEffects(Vector3 hitPoint, Vector3 hitNormal, SurfaceEffectSO surfaceEffectSo,
            float soundOffset)
        {
            foreach (SpawnObjectEffectSO spawnObjectEffect in surfaceEffectSo.SpawnObjectEffects)
            {
                if (spawnObjectEffect.Probability > Random.value)
                {
                    GameObject instance = LeanPool.Spawn(spawnObjectEffect.Prefab, hitPoint + hitNormal * 0.001f,
                        Quaternion.LookRotation(hitNormal));

                    instance.transform.forward = hitNormal;

                    if (spawnObjectEffect.RandomizeRotation)
                    {
                        Vector3 offset = new Vector3(
                            Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.x),
                            Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.y),
                            Random.Range(0, 180 * spawnObjectEffect.RandomizedRotationMultiplier.z)
                        );

                        instance.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + offset);
                    }
                }
            }

            foreach (PlayAudioEffectSO playAudioEffect in surfaceEffectSo.PlayAudioEffects)
            {
                if (playAudioEffect.AudioClips.Count > 0)
                {
                    AudioClip clip = playAudioEffect.AudioClips[Random.Range(0, playAudioEffect.AudioClips.Count)];
                    GameObject audioPrefab = playAudioEffect.AudioSourcePrefab.gameObject;
                    GameObject audioInstance = LeanPool.Spawn(audioPrefab, hitPoint, Quaternion.identity);
                    AudioSource audioSource = audioInstance.GetComponent<AudioSource>();

                    audioSource.PlayOneShot(clip,
                        soundOffset * Random.Range(playAudioEffect.VolumeRange.x, playAudioEffect.VolumeRange.y));

                    StartCoroutine(DisableAudioSource(audioSource, clip.length));
                }
            }
        }

        private IEnumerator DisableAudioSource(AudioSource audioSource, float time)
        {
            yield return new WaitForSeconds(time);

            LeanPool.Despawn(audioSource.gameObject);
        }

        private class TextureAlpha
        {
            public float Alpha;
            public Texture Texture;
        }
    }
}