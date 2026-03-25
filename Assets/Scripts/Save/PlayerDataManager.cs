using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [SerializeField] private string playerName;
    [SerializeField] private int academicScore;
    [SerializeField] private int stress;
    [SerializeField] private int money;
    [SerializeField] private int currentDay;
    [SerializeField] private int failedExams;

    private Dictionary<string, int> npcAffinity = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStats()
    {
        playerName = "";
        academicScore = 0;
        stress = 0;
        money = 5_000_000;
        currentDay = 1;
        failedExams = 0;

        // Khởi tạo affinity cho tất cả NPC
        npcAffinity["Harry"] = 0;
        npcAffinity["HungBig"] = 0;
        npcAffinity["HungSmall"] = 0;
        npcAffinity["Jessica"] = 0;
        npcAffinity["Joli"] = 0;
        npcAffinity["KieuVy"] = 0;
        npcAffinity["MinhAnh"] = 0;
        npcAffinity["MrTam"] = 0;
        npcAffinity["NhanLe"] = 0;
        npcAffinity["QuanShiba"] = 0;
        npcAffinity["Thong"] = 0;
        npcAffinity["TonyThang"] = 0;
        npcAffinity["UyenChi"] = 0;
    }

    // ========== Getter Methods ==========
    public string GetPlayerName() => playerName;
    public int GetAcademicScore() => academicScore;
    public int GetStress() => stress;
    public int GetMoney() => money;
    public int GetCurrentDay() => currentDay;
    public int GetFailedExams() => failedExams;
    public int GetNPCAffinity(string npcName) => npcAffinity.ContainsKey(npcName) ? npcAffinity[npcName] : 0;

    // ========== Setter Methods ==========
    public void SetPlayerName(string name) => playerName = name;
    public void SetAcademicScore(int score) => academicScore = Mathf.Max(0, score);
    public void SetStress(int value) => stress = Mathf.Clamp(value, 0, 100);
    public void SetMoney(int amount) => money = Mathf.Max(0, amount);
    public void SetCurrentDay(int day) => currentDay = Mathf.Max(1, day);
    public void SetFailedExams(int count) => failedExams = Mathf.Max(0, count);

    public void AddNPCAffinity(string npcName, int amount)
    {
        if (npcAffinity.ContainsKey(npcName))
        {
            npcAffinity[npcName] = Mathf.Min(100, npcAffinity[npcName] + amount);
        }
    }

    // ========== Save/Load Integration ==========
    public void LoadFromSaveData(SaveData data)
    {
        playerName = data.playerName;
        academicScore = data.academicScore;
        stress = data.stress;
        money = data.money;
        currentDay = data.currentDay;
        failedExams = data.failedExams;

        npcAffinity["Harry"] = data.aff_Harry;
        npcAffinity["HungBig"] = data.aff_HungBig;
        npcAffinity["HungSmall"] = data.aff_HungSmall;
        npcAffinity["Jessica"] = data.aff_Jessica;
        npcAffinity["Joli"] = data.aff_Joli;
        npcAffinity["KieuVy"] = data.aff_KieuVy;
        npcAffinity["MinhAnh"] = data.aff_MinhAnh;
        npcAffinity["MrTam"] = data.aff_MrTam;
        npcAffinity["NhanLe"] = data.aff_NhanLe;
        npcAffinity["QuanShiba"] = data.aff_QuanShiba;
        npcAffinity["Thong"] = data.aff_Thong;
        npcAffinity["TonyThang"] = data.aff_TonyThang;
        npcAffinity["UyenChi"] = data.aff_UyenChi;
    }

    public void SaveToData(SaveData data)
    {
        data.playerName = playerName;
        data.academicScore = academicScore;
        data.stress = stress;
        data.money = money;
        data.currentDay = currentDay;
        data.failedExams = failedExams;

        data.aff_Harry = GetNPCAffinity("Harry");
        data.aff_HungBig = GetNPCAffinity("HungBig");
        data.aff_HungSmall = GetNPCAffinity("HungSmall");
        data.aff_Jessica = GetNPCAffinity("Jessica");
        data.aff_Joli = GetNPCAffinity("Joli");
        data.aff_KieuVy = GetNPCAffinity("KieuVy");
        data.aff_MinhAnh = GetNPCAffinity("MinhAnh");
        data.aff_MrTam = GetNPCAffinity("MrTam");
        data.aff_NhanLe = GetNPCAffinity("NhanLe");
        data.aff_QuanShiba = GetNPCAffinity("QuanShiba");
        data.aff_Thong = GetNPCAffinity("Thong");
        data.aff_TonyThang = GetNPCAffinity("TonyThang");
        data.aff_UyenChi = GetNPCAffinity("UyenChi");
    }

    }