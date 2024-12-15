using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterSkinHandler : MonoBehaviour
  {
    [SerializeField] private bool randomSkin;
    [SerializeField] private GameObject[] skins;

    private void Start()
    {
      EnableRandomSkin();
    }

    private void EnableRandomSkin()
    {
      if (randomSkin == false) return;

      foreach (var skin in skins)
      {
        skin.SetActive(false);
      }

      skins[Random.Range(0, skins.Length)].SetActive(true);
    }
  }
}