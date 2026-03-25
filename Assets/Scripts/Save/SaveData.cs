using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public string currentScene;
    public int academicScore;
    public int stress;
    public int money;
    public int currentDay;
    public int failedExams;  // Cúp học

    // Thiện cảm từng NPC
    public int aff_Harry;
    public int aff_HungBig;
    public int aff_HungSmall;
    public int aff_Jessica;
    public int aff_Joli;
    public int aff_KieuVy;
    public int aff_MinhAnh;
    public int aff_MrTam;
    public int aff_NhanLe;
    public int aff_QuanShiba;
    public int aff_Thong;
    public int aff_TonyThang;
    public int aff_UyenChi;

    // Constructor để khởi tạo giá trị mặc định
    public SaveData()
    {
        playerName = "";
        currentScene = "Scene_Map001_Home";
        academicScore = 0;
        stress = 0;
        money = 5_000_000;
        currentDay = 1;
        failedExams = 0;
        aff_Harry = 0;
        aff_HungBig = 0;
        aff_HungSmall = 0;
        aff_Jessica = 0;
        aff_Joli = 0;
        aff_KieuVy = 0;
        aff_MinhAnh = 0;
        aff_MrTam = 0;
        aff_NhanLe = 0;
        aff_QuanShiba = 0;
        aff_Thong = 0;
        aff_TonyThang = 0;
        aff_UyenChi = 0;
    }
}