using UnityEngine;
using UniLinq;
using System.IO;
using System.Collections.Generic;

namespace VABOrganizer
{
  /// <summary>
  /// Main addon startup. Loads the definitions when the part loader finishes, at the end
  /// of the loading screen, so they exist before any game scene whichever one comes first.
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class VABOrganizer : MonoBehaviour
  {
    public static VABOrganizer Instance { get; private set; }
    public void Awake()
    {
      GameEvents.OnPartLoaderLoaded.Add(OnPartLoaderLoaded);
    }

    public void OnDestroy()
    {
      GameEvents.OnPartLoaderLoaded.Remove(OnPartLoaderLoaded);
    }

    private void OnPartLoaderLoaded()
    {
      Settings.Load();
      AdvancedSortingData.Load();
      SubcategorySorting.Load();
    }
  }

  /// <summary>
  /// Manange loading up the assets
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class VABOrganizerAssets : MonoBehaviour
  {
    public static Dictionary<string,Sprite> Sprites { get; private set; }

    public static GameObject AdvancedSortPrefab { get; private set; }

    internal static string ASSET_PATH = "GameData/VABOrganizer/Assets/vaborganizer.dat";
    internal static string SPRITE_ATLAS_NAME = "vab-organizer";

    public static Sprite GetSprite(string key)
    {
      if (Sprites.ContainsKey(key))
      {
        return Sprites[key]; 
      }
      Debug.LogWarning($"[Assets] Could not find sprite {key}");
      return Sprites.First().Value;
    }

    private void Awake()
    {
      GameObject.DontDestroyOnLoad(gameObject);
      Utils.Log("[Assets]: Loading UI Prefabs");
      AssetBundle prefabs = AssetBundle.LoadFromFile(Path.Combine(KSPUtil.ApplicationRootPath, ASSET_PATH));

      AdvancedSortPrefab = prefabs.LoadAsset("OrganizerFilters") as GameObject;
      AdvancedSortPrefab.AddComponent<UIAdvancedSorterWidget>().AssignReferences();
      AdvancedSortPrefab.transform.SetParent(this.transform);

      Sprite[] spriteSheet = prefabs.LoadAssetWithSubAssets<Sprite>(SPRITE_ATLAS_NAME);
      Sprites = new Dictionary<string, Sprite>();

      foreach (Sprite subSprite in spriteSheet)
      {
        Sprites.Add(subSprite.name, subSprite);
      }
      
      Utils.Log("[Assets]: Loaded UI Prefabs");
    }
  }
}
