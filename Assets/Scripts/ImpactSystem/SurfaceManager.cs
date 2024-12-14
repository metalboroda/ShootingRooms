using Assets.Scripts.ImpactSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimatePooling;

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

  public void HandleImpact(GameObject HitObject, Vector3 HitPoint, Vector3 HitNormal, ImpactTypeSO Impact, int TriangleIndex)
  {
    if (HitObject.TryGetComponent(out Terrain terrain))
    {
      List<TextureAlpha> activeTextures = GetActiveTexturesFromTerrain(terrain, HitPoint);

      foreach (TextureAlpha activeTexture in activeTextures)
      {
        SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture.Texture);

        if (surfaceType != null)
        {
          foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
          {
            if (typeEffect.ImpactType == Impact)
            {
              PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, activeTexture.Alpha);
            }
          }
        }
        else
        {
          foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in defaultSurface.ImpactTypeEffects)
          {
            if (typeEffect.ImpactType == Impact)
            {
              PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
            }
          }
        }
      }
    }
    else if (HitObject.TryGetComponent(out Renderer renderer))
    {
      Texture activeTexture = GetActiveTextureFromRenderer(renderer, TriangleIndex);
      SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture);

      if (surfaceType != null)
      {
        foreach (SurfaceSO.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.ImpactTypeEffects)
        {
          if (typeEffect.ImpactType == Impact)
          {
            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
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
          if (typeEffect.ImpactType == Impact)
          {
            PlayEffects(HitPoint, HitNormal, typeEffect.SurfaceEffect, 1);
          }
        }
      }
    }
  }

  private List<TextureAlpha> GetActiveTexturesFromTerrain(Terrain Terrain, Vector3 HitPoint)
  {
    Vector3 terrainPosition = HitPoint - Terrain.transform.position;
    Vector3 splatMapPosition = new Vector3(
      terrainPosition.x / Terrain.terrainData.size.x,
      0,
      terrainPosition.z / Terrain.terrainData.size.z
    );

    int x = Mathf.FloorToInt(splatMapPosition.x * Terrain.terrainData.alphamapWidth);
    int z = Mathf.FloorToInt(splatMapPosition.z * Terrain.terrainData.alphamapHeight);

    float[,,] alphaMap = Terrain.terrainData.GetAlphamaps(x, z, 1, 1);

    List<TextureAlpha> activeTextures = new List<TextureAlpha>();

    for (int i = 0; i < alphaMap.Length; i++)
    {
      if (alphaMap[0, 0, i] > 0)
      {
        activeTextures.Add(new TextureAlpha()
        {
          Texture = Terrain.terrainData.terrainLayers[i].diffuseTexture,
          Alpha = alphaMap[0, 0, i]
        });
      }
    }

    return activeTextures;
  }

  private Texture GetActiveTextureFromRenderer(Renderer Renderer, int TriangleIndex)
  {
    if (Renderer.TryGetComponent(out MeshFilter meshFilter))
    {
      Mesh mesh = meshFilter.mesh;

      if (mesh.subMeshCount > 1)
      {
        int[] hitTriangleIndices = new int[]
        {
          mesh.triangles[TriangleIndex * 3],
          mesh.triangles[TriangleIndex * 3 + 1],
          mesh.triangles[TriangleIndex * 3 + 2]
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
              return Renderer.sharedMaterials[i].mainTexture;
            }
          }
        }
      }
      else
      {
        return Renderer.sharedMaterial.mainTexture;
      }
    }

    return null;
  }

  private void PlayEffects(Vector3 HitPoint, Vector3 HitNormal, SurfaceEffectSO SurfaceEffect, float SoundOffset)
  {
    foreach (SpawnObjectEffectSO spawnObjectEffect in SurfaceEffect.SpawnObjectEffects)
    {
      if (spawnObjectEffect.Probability > Random.value)
      {
        GameObject instance = UltimatePool.spawn(spawnObjectEffect.Prefab, HitPoint + HitNormal * 0.001f, Quaternion.LookRotation(HitNormal));

        instance.transform.forward = HitNormal;

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

    foreach (PlayAudioEffectSO playAudioEffect in SurfaceEffect.PlayAudioEffects)
    {
      if (playAudioEffect.AudioClips.Count > 0)
      {
        AudioClip clip = playAudioEffect.AudioClips[Random.Range(0, playAudioEffect.AudioClips.Count)];
        GameObject audioPrefab = playAudioEffect.AudioSourcePrefab.gameObject;
        GameObject audioInstance = UltimatePool.spawn(audioPrefab, HitPoint, Quaternion.identity);
        AudioSource audioSource = audioInstance.GetComponent<AudioSource>();

        audioSource.PlayOneShot(clip, SoundOffset * Random.Range(playAudioEffect.VolumeRange.x, playAudioEffect.VolumeRange.y));

        StartCoroutine(DisableAudioSource(audioSource, clip.length));
      }
    }
  }

  private IEnumerator DisableAudioSource(AudioSource AudioSource, float Time)
  {
    yield return new WaitForSeconds(Time);

    UltimatePool.despawn(AudioSource.gameObject);
  }

  private class TextureAlpha
  {
    public float Alpha;
    public Texture Texture;
  }
}