using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud.gameObject);
            Destroy(menu.gameObject);
            return;
        }

        PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    ///Ressources
    public List<Sprite> playerSprite;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrice;
    public List<int> xpTable;

    ///References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public Animator deathMenuAnim;
    public GameObject hud;
    public GameObject menu;

    //Logic
    public int pesos;
    public int experience;


    //Floating Text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg,fontSize,color,position,motion,duration);
    }

    //Upgrade weapon

    public bool TryUpgardeWeapon()
    {
        //is the weapon max leve?
        if (weaponPrice.Count <= weapon.weaponLevel)
            return false;

        if (pesos >= weaponPrice[weapon.weaponLevel])
        {
            pesos -= weaponPrice[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }


    //EXP system
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;
        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) // max lvl
                return r;
        }

        return r;
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;
        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            player.OnLevelUp();
    }

    //deathmenu and respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
    }

    //ON Scene Loaded
    public void OnSceneLoaded(Scene s,LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    //Save
    /*
     * Int preferedSkin
     * Int pesos
     * Int exp
     * INT weaponlvl
     * 
     */
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        PlayerPrefs.SetString("SaveState", s);
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //Change player skin
        pesos = int.Parse(data[1]);

        //exp
        experience = int.Parse(data[2]);
        if(GetCurrentLevel() != 1)
        player.SetLevel(GetCurrentLevel());

        //Change the weapon level
        weapon.SetWeaponLevel(int.Parse(data[3]));
    }
}
