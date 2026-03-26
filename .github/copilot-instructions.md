# FPT Adventure – GitHub Copilot Agent Instructions

> **Mục đích file này:** Cung cấp toàn bộ context về dự án game **FPT Adventure** để Copilot Agent trong Visual Studio hiểu đúng cấu trúc, logic game, quy ước đặt tên, và có thể hỗ trợ code, debug, generate script một cách chính xác.

---

## 📌 Tổng Quan Dự Án

| Thuộc tính | Giá trị |
|---|---|
| **Tên game** | FPT Adventure |
| **Engine** | Unity 6 (`6000.3.11f1`) |
| **Ngôn ngữ** | C# 9.0 / .NET Standard 2.1 |
| **Thể loại** | Life Simulation / Visual Novel RPG 2D |
| **Camera** | Top-down 2D |
| **Platform** | PC |
| **Input** | Both (Legacy Input + New Input System) – `activeInputHandler: 2` |
| **IDE** | Visual Studio Community 2026 |
| **Git branch chính** | `main` |
| **Remote** | `https://github.com/TEAMFGU/FGU301_TEAM4` |

**Mô tả:** Game mô phỏng cuộc sống sinh viên mới nhập học ĐH FPT (Thủ Đức, TP.HCM). Người chơi trải qua daily loop gồm học tập, kết bạn, quản lý tài chính và stress, dẫn đến nhiều kết thúc khác nhau tùy theo lựa chọn.

---

## 🗂️ Cấu Trúc Thư Mục Hiện Tại

```
Assets/
├── Animation/                    # Animator Controllers + Animation Clips
│   ├── Player/                   # Walk_Front/Back/Left/Right.anim, Idle_*.anim
│   │   └── PlayerAnimator.controller   # ← Animator chính của Player
│   ├── Harry_Hai/
│   ├── HungBig/
│   ├── HungSmall/
│   ├── Jessica/
│   ├── Joli/
│   ├── KieuVy/
│   ├── MinhAnh/
│   ├── Mr.Tam/
│   ├── NhanLe/
│   ├── QuanShiba/
│   ├── Thong/
│   ├── Tony_Thang/
│   └── Uyen_Chi/
├── Canvas/
│   └── Canvas.prefab
├── Prefabs/
│   ├── Player.prefab
│   ├── Harry.prefab
│   ├── HungBig.prefab
│   ├── HungSmall.prefab
│   ├── Jessica.prefab
│   ├── Joli.prefab
│   ├── KieuVy.prefab
│   ├── MinhAnh.prefab
│   ├── Mr.Tam.prefab
│   ├── NhanLe.prefab
│   ├── QuanShiba.prefab
│   ├── Thong.prefab
│   ├── Tony.prefab
│   └── UyenChi.prefab
├── Scenes/
│   ├── BIDA.unity
│   ├── BusStation.unity
│   ├── Carteria.unity
│   ├── Class.unity
│   ├── EatWall.unity
│   ├── Home.unity
│   ├── Lobby.unity
│   ├── Park.unity
│   ├── Scene_Map001_Home.unity    # ← Scene chính đang phát triển
│   ├── schoolyard.unity
│   ├── Streets.unity
│   └── Cutsence0/
│       ├── Map00_BenXe.unity      # Cutscene intro bến xe
│       ├── Map000_Strange_guy.unity
│       └── IntroCutscene.cs       # Script cutscene intro
├── ScriptableObjects/
│   └── NPC/
│       ├── NPC_Harry.asset
│       ├── NPC_HungBig.asset
│       ├── NPC_HungSmall.asset
│       ├── NPC_Jessica.asset
│       ├── NPC_Joli.asset
│       ├── NPC_KieuVy.asset
│       ├── NPC_MinhAnh.asset
│       ├── NPC_MrTam.asset
│       ├── NPC_NhanLe.asset
│       ├── NPC_QuanShiba.asset
│       ├── NPC_Thong.asset
│       ├── NPC_TonyThang.asset
│       └── NPC_UyenChi.asset
├── Scripts/
│   ├── InteractionManager.cs      # Singleton – xử lý Z/X input
│   ├── MenuHandler.cs             # Khởi tạo managers + chuyển scene từ Start Menu
│   ├── PlayerMovement.cs          # Di chuyển WASD, canMove lock, Animator Blend Tree
│   ├── SpawnPoint.cs              # Spawn player đúng vị trí khi chuyển scene
│   ├── TeleportPortal.cs          # Chọn điểm đến + chuyển scene
│   ├── NPC/
│   │   ├── NPCData.cs             # ScriptableObject – dữ liệu NPC
│   │   └── NPC_Interactor.cs      # Hệ thống tương tác + choices + cutscene mốc
│   ├── Save/
│   │   ├── PlayerDataManager.cs   # Singleton – lưu trữ stats + NPC affinity in-memory
│   │   ├── SaveData.cs            # [Serializable] class chứa toàn bộ dữ liệu save
│   │   └── SaveSystem.cs          # Singleton – đọc/ghi JSON file save
│   └── UI/
│       ├── DayManager.cs          # Singleton – quản lý currentDay + TimeOfDay + AdvanceTime()
│       ├── DialogueManager.cs     # Singleton – hộp thoại + typing effect + choices UI
│       ├── NameInputHandler.cs    # Xử lý panel nhập tên ở StartMenu
│       └── UIManager.cs           # Singleton – HUD ngày (sprite buổi + số ngày)
└── Sprites/
    └── NPC/                       # Sprite sheets cho tất cả NPC + Easter Egg
```

---

## 📜 Scripts Hiện Có & Chức Năng

### PlayerMovement.cs
- Di chuyển WASD (1 hướng, RPG style)
- Dùng `Rigidbody2D.linearVelocity` (Unity 6 API)
- Animator dùng **2 Blend Tree** riêng (Idle + Walk) với parameters: `Horizontal`, `Vertical`, `IsMoving`
- `lastDir` lưu hướng cuối → khi dừng, Idle animation quay đúng hướng
- `[HideInInspector] public bool canMove = true` – `DialogueManager` set `false` khi đang chọn option, `true` khi chọn xong

### InteractionManager.cs
- Singleton (`Instance`)
- `Z` = tương tác / xác nhận
- `X` = hủy / mở menu
- Flag `isInteracting` để quản lý trạng thái

### NPCData.cs (ScriptableObject)
- `[CreateAssetMenu(menuName = "FPT Adventure/NPC Data")]`
- Fields: `npcName`, `description`, `faceSprite`, `initialAffection`, `routeType`, `level` (1-4), `affection` (0-100)
- Cutscene flags: `unlockedCutscene25`, `unlockedCutscene50`, `unlockedCutscene75`, `unlockedCutscene100`

### NPC_Interactor.cs
- Gắn vào mỗi NPC GameObject
- Khi `Interact()` → hiện dialogue → sau dialogue hiện choices (nếu có) → apply stats
- **`ChoiceEffect`**: class con chứa `affectionChange`, `stressChange`, `studyChange` cho MỖI lựa chọn
- **`DialogueSlot`**: `npcText`, `playerChoices[]`, `choiceEffects[]` (index khớp choices), fallback flat stats
- `ApplyStatsAndProgress(slot, choiceIndex)`: nếu `choiceIndex >= 0` → dùng `choiceEffects[i]`, ngược lại → dùng fallback
- `currentLevel` 1→4, tăng mỗi lần tương tác
- 4 mốc cutscene: 25%, 50%, 75%, 100% → trigger `UnityEvent` 1 lần duy nhất
- **Loại bỏ chỉ số Money khỏi NPC_Interactor**. `ChoiceEffect` chỉ có: `affectionChange`, `stressChange`, `studyChange`

### DialogueManager.cs (Singleton, DontDestroyOnLoad)
- Quản lý UI hộp thoại: `dialoguePanel`, `avatarImage`, `nameTagText`, `messageText`, `hintText`
- **Typing effect**: hiển thị text từng ký tự, nhấn `Z` skip → hiện full text ngay
- **Choices UI**: `choicePanel` chứa `optionRows[]`, `cursorImages[]` (Btn_Play sprite), `optionTexts[]`
- `ShowDialogue(npcData, lines, onComplete, hasChoicesAfter)` – hiện dialogue, `hasChoicesAfter` điều khiển hintText dòng cuối
- `ShowChoices(choices[], callback)` – hiện bảng chọn, khóa `PlayerMovement.canMove`, W/S di chuyển cursor, Z xác nhận
- `ConfirmChoice()` → mở lại `canMove`, gọi callback với index lựa chọn
- `{playerName}` và `{player}` trong dialogue → replace runtime bằng `PlayerPrefs.GetString("PlayerName")`

### DayManager.cs (Singleton, DontDestroyOnLoad)
- `currentDay` (int, bắt đầu 1), `currentTime` (`TimeOfDay` enum: Morning / Afternoon / Evening)
- `AdvanceTime()`: Sáng → Chiều → Tối → Ngày mới (Sáng, `currentDay++`)
- `event Action onTimeChanged` – `UIManager` subscribe để cập nhật HUD
- `SetDayTime(day, time)` – gọi từ `SaveSystem` khi load game

### UIManager.cs (Singleton, DontDestroyOnLoad)
- HUD góc trái trên: `dayCountText` (số ngày) + `sessionImage` (sprite buổi: sáng/chiều/tối)
- Subscribe `DayManager.onTimeChanged` → gọi `UpdateDayUI()` tự động
- Sprite buổi: `morningSprite`, `afternoonSprite`, `eveningSprite` – gán trong Inspector

### PlayerDataManager.cs (Singleton, DontDestroyOnLoad)
- Lưu trữ in-memory: `playerName`, `academicScore`, `stress`, `money`, `currentDay`, `failedExams`
- `Dictionary<string, int> npcAffinity` – 13 NPC key, giá trị 0-100
- Getter/Setter: `GetAcademicScore()`, `SetStress(value)`, `AddNPCAffinity(name, amount)`, v.v.
- `SaveToData(SaveData)` / `LoadFromSaveData(SaveData)` – bridge với SaveSystem

### SaveData.cs
- `[System.Serializable]` class – serializable bằng `JsonUtility`
- Fields: `playerName`, `currentScene`, `academicScore`, `stress`, `money`, `currentDay`, `failedExams`
- 13 field NPC affinity: `aff_Harry`, `aff_HungBig`, ..., `aff_UyenChi`

### SaveSystem.cs (Singleton, DontDestroyOnLoad)
- `SaveGame()`: đọc từ `PlayerDataManager.SaveToData()` → serialize JSON → ghi file `Assets/Save/savegame.json`
- `LoadGame()`: đọc file JSON → `PlayerDataManager.LoadFromSaveData()` → return `true/false`
- `HasSaveFile()`, `DeleteSaveFile()`, `GetCurrentSaveData()`

### MenuHandler.cs
- Scene StartMenu: gọi `NameInputHandler.OpenNameInputPanel()` khi nhấn "Chơi Mới"
- `InitializePersistentManagers()`: Instantiate prefabs `DayManager`, `UIManager`, `DialogueManager` nếu chưa tồn tại
- `LoadNextScene()` → `SceneManager.LoadScene("Map00_BenXe")` (dùng tên scene cố định)
- `ContinueGame()` → gọi `SaveSystem.Instance.LoadGame()`

### NameInputHandler.cs
- Panel nhập tên: `TMP_InputField` + nút Xác nhận + nút Hủy
- Validate tên không rỗng → `PlayerPrefs.SetString("PlayerName", ...)` → gọi `MenuHandler.InitializePersistentManagers()` → `SceneManager.LoadScene("Map00_BenXe")`

### TeleportPortal.cs
- Nhấn `E` mở menu chọn điểm đến
- Dùng `PlayerPrefs` lưu `SpawnPoint` ID trước khi `LoadScene`
- `SpawnPoint.cs` đọc ID để đặt Player đúng vị trí

---

## 🎬 Scene / Map

| Scene File | Map | Mô tả |
|---|---|---|
| `Map00_BenXe.unity` | Map0 | Cutscene intro – 3 NPC xe ôm rượt đuổi |
| `Map000_Strange_guy.unity` | Map00 | Cutscene – NPC môi giới nhà trọ |
| `Scene_Map001_Home.unity` | Map01 | Phòng trọ – Hub chính (đang phát triển) |
| `schoolyard.unity` | Map02 | Sân trường FPT |
| `Lobby.unity` | Map03 | Sảnh / Hành lang |
| `Class.unity` | Map04 | Lớp học |
| `Carteria.unity` | Map05 | Căn tin |
| `Park.unity` | Map06 | Công viên |
| `BIDA.unity` | Map07 | Phòng Bida (BingBoong) |
| `Streets.unity` | | Đường phố |
| `Home.unity` | | Scene home cũ |
| `BusStation.unity` | | Bến xe buýt |
| `EatWall.unity` | | Quán ăn |

### Quy tắc chuyển Scene
- Dùng `TeleportPortal` (nhấn `E`) hoặc `TriggerZone` (Box Collider 2D – Is Trigger)
- Lưu `SpawnPoint` ID qua `PlayerPrefs` trước khi `SceneManager.LoadScene()`
- `SpawnPoint.cs` tại scene đích đọc ID và teleport Player đến đúng vị trí

---

## 📊 Hệ Thống Chỉ Số (Stats System) – Triển Khai

### PlayerDataManager.cs (Singleton, DontDestroyOnLoad) – Lưu Trữ In-Memory
```csharp
// Thực tế trong code:
[SerializeField] private string playerName;
[SerializeField] private int academicScore;   // Học lực: 0–∞, cần >= 100 để pass
[SerializeField] private int stress;           // Stress: 0–100, >= 80 warning
[SerializeField] private int money;            // Tiền VNĐ (lưu trữ nhưng không dùng trong NPC interaction)
[SerializeField] private int currentDay;       // Ngày hiện tại (bắt đầu 1)
[SerializeField] private int failedExams;      // Số lần cúp học
private Dictionary<string, int> npcAffinity;   // 13 NPC, giá trị 0-100
```

### Luồng dữ liệu: NPC Interaction → Stats
```
Player nhấn Z → NPC_Interactor.Interact()
  → DialogueManager.ShowDialogue(lines, onComplete)
    → onComplete: OnDialogueFinished(slot)
      → Nếu có choices: DialogueManager.ShowChoices(choices, callback)
        → callback: ApplyStatsAndProgress(slot, choiceIndex)
      → Nếu không: ApplyStatsAndProgress(slot, -1)
        → Đọc choiceEffects[i] hoặc fallback stats
        → PlayerDataManager.SetStress/SetAcademicScore/AddNPCAffinity
        → data.affection += delta (trên NPCData ScriptableObject)
        → CheckAffectionMilestones() → trigger UnityEvent
        → currentLevel++ (max 4)
```

### Bảng Thay Đổi Chỉ Số (Cấu Hình Trong Inspector của NPC_Interactor)

| Loại | Nơi cấu hình | Mô tả |
|---|---|---|
| **Không có choices** | `DialogueSlot` fallback fields | `affectionChange`, `stressChange`, `studyChange` áp dụng ngay sau dialogue |
| **Có choices** | `ChoiceEffect[]` (index khớp `playerChoices[]`) | Mỗi lựa chọn có bộ stats riêng. Player chọn option nào → áp dụng `choiceEffects[nào]` |

### Ví dụ Inspector Setup (Harry hỏi đi bida)
```
Dialogue Levels [0]
├─ Npc Text: "Ê cu đi đâu đây? Đi bida không?"
├─ Player Choices: ["Ư, đi thôi!", "Thôi, mình bận rồi."]
├─ Choice Effects:
│    [0] Affection: +10, Stress: -20, Study: -5   ← Đi bida
│    [1] Affection: +5,  Stress: -5,  Study: +5    ← Từ chối
└─ Fallback: (để 0 vì có choiceEffects)
```

> **Khi NPC đã được tương tác trong ngày hoặc điểm thiện cảm đạt tối đa**, thoại chung của tất cả NPC là `[.............................]`

---

## 🏁 Hệ Thống Ending

GamePlay sẽ bao gồm 30 ngày, mỗi ngày 3 buổi (Sáng, Chiều, Tối). Sau ngày 30, hệ thống sẽ tự động kiểm tra điều kiện để quyết định ending nào sẽ trigger.

| Ending | Điều kiện |
|---|---|
| **BAD END** | Cúp học 4 lần **HOẶC** Học lực cuối cùng < 100 |
| **NORMAL END** | Học lực > 100 nhưng có NPC chưa đạt 100 điểm thiện cảm |
| **HAPPY ENDING** | Xem chi tiết bên dưới |

### 🏆 HAPPY ENDING – Điều Kiện Chi Tiết (7 NPC)

1. **Học lực ≥ 150**
2. **Cúp học < 4 lần**
3. **Tất cả 7 NPC** (Quân, Hưng, Chi, Joli, Harry, Nhân, Minh Anh/Thắng) đều đạt **100/100** thiện cảm
   - Ngoại trừ: **Jessica tối đa 50** (sau scene 2 của Jessica không hiện điểm thiện cảm tối đa nữa)
4. **Điểm thiện cảm Thông phải thấp (< 30)** – để chứng minh bạn chọn bạn tốt để chơi

---

## 👥 Danh Sách NPC

### NPC Đặc Biệt (Có Route Riêng)

| NPC ID | Tên | Route | Prefab | ScriptableObject |
|---|---|---|---|---|
| `npc_thong` | Thông | Bad End | `Thong.prefab` | `NPC_Thong.asset` |
| `npc_minhanh` | Minh Anh | Good End | `MinhAnh.prefab` | `NPC_MinhAnh.asset` |
| `npc_kieuvy` | Kiều Vy | Dead End | `KieuVy.prefab` | `NPC_KieuVy.asset` |

### NPC Phụ (Support Happy Ending)

| NPC ID | Tên | Nhóm | Prefab | ScriptableObject |
|---|---|---|---|---|
| `npc_quanshiba` | Quân Shiba | Tứ Đại | `QuanShiba.prefab` | `NPC_QuanShiba.asset` |
| `npc_hunglon` | Hưng Lớn | Tứ Đại | `HungBig.prefab` | `NPC_HungBig.asset` |
| `npc_joli` | Joli | Tứ Đại | `Joli.prefab` | `NPC_Joli.asset` |
| `npc_uyenchi` | Uyển Chi | Tứ Đại | `UyenChi.prefab` | `NPC_UyenChi.asset` |
| `npc_mrtam` | Mr. Tam | Giảng viên | `Mr.Tam.prefab` | `NPC_MrTam.asset` |
| `npc_nhanle` | Nhân Lê | Độc lập | `NhanLe.prefab` | `NPC_NhanLe.asset` |
| `npc_harry` | Harry | Độc lập | `Harry.prefab` | `NPC_Harry.asset` |
| `npc_tonythang` | Tony Thắng | Cặp đôi | `Tony.prefab` | `NPC_TonyThang.asset` |
| `npc_jessica` | Jessica | Cặp đôi | `Jessica.prefab` | `NPC_Jessica.asset` |
| `npc_hungnho` | Hưng Nhỏ | Độc lập | `HungSmall.prefab` | `NPC_HungSmall.asset` |

### NPC Easter Egg (Static sprite, không cần animation)

| NPC | Nơi xuất hiện | Sprite |
|---|---|---|
| Tống Trần Lê Gay | Nhà vệ sinh | `TongTranLeGay.png` |
| Thanh Sơn | Sân trường | `ThanhSon.png` |
| Daniel Tín | Hành lang | `DanielTin.png` |
| Cá Ngừ | Random | `CaNgu.png` |
| Trân | Lớp học | `Tran.png` |
| Vân Khánh | Hành lang | `VanKhanh.png` |
| Hội | Sân trường | `Hoi.png` |

---

## 🎮 Controls & Input

| Phím | Hành động |
|---|---|
| `W A S D` / `↑ ↓ ← →` | Di chuyển nhân vật |
| `Z` | Tương tác / Xác nhận / Tiếp tục thoại |
| `X` | Hủy tương tác / Mở menu |
| `E` | Mở TeleportPortal menu |
| `Escape` | Pause menu / Đóng portal |

> **Input:** `activeInputHandler: 2` (Both) – code hiện dùng Legacy `Input.GetKeyDown()`.

---

## 🎞️ Animator Player

Player dùng **2 Blend Tree** riêng biệt trong `PlayerAnimator.controller`:

- **Blend Tree Idle**: 4 clip (`Idle_Front`, `Idle_Back`, `Idle_Left`, `Idle_Right`)
- **Blend Tree Walk**: 4 clip (`Walk_Front`, `Walk_Back`, `Walk_Left`, `Walk_Right`)
- **Transitions**: `IsMoving = true` → Walk, `IsMoving = false` → Idle
- **Blend Type**: 2D Freeform Directional, Parameters: X = `Horizontal`, Y = `Vertical`
- **Has Exit Time**: OFF, **Transition Duration**: 0

### Naming convention cho animation:
- Walk_Front = đi lên (W), Walk_Back = đi xuống (S)
- Idle_Front = idle nhìn lên, Idle_Back = idle nhìn xuống

---

## 🗃️ Quy Ước Đặt Tên (Naming Conventions)

### Script

| Loại | Convention | Ví dụ |
|---|---|---|
| MonoBehaviour | PascalCase | `NPC_Interactor`, `InteractionManager` |
| ScriptableObject | PascalCase | `NPCData` |
| Manager/Singleton | `[Tên]Manager` | `InteractionManager`, `PlayerDataManager` |
| Enum | PascalCase | `NPCRouteType`, `StatType` |
| Private field | camelCase | `currentLevel`, `lastDir` |
| Public field | camelCase | `moveSpeed`, `npcName` |

### Asset

| Loại | Convention | Ví dụ |
|---|---|---|
| NPC Prefab | `[TenNPC].prefab` | `Harry.prefab`, `NhanLe.prefab` |
| ScriptableObject | `NPC_[TenNPC].asset` | `NPC_Harry.asset` |
| Scene | Tên map mô tả | `Scene_Map001_Home.unity`, `BIDA.unity` |
| Animation Clip | `[Action]_[Direction].anim` | `Walk_Front.anim`, `Idle_Back.anim` |
| NPC Sprite | `[TenNPC].png` | `Harry.png`, `CaNgu.png` |

### Dialogue Placeholder
- Dùng `{playerName}` trong dialogue text → replace runtime bằng `PlayerPrefs.GetString("PlayerName")`

---

## 🧠 Managers (Đã có / Chưa có)

| Manager | Trạng thái | Chức năng |
|---|---|---|
| `InteractionManager` | ✅ Đã có | Singleton, xử lý Z/X input |
| `PlayerDataManager` | ✅ Đã có | Singleton, DontDestroyOnLoad, lưu stats + NPC affinity in-memory |
| `DialogueManager` | ✅ Đã có | Singleton, DontDestroyOnLoad, hộp thoại + typing effect + choices UI |
| `DayManager` | ✅ Đã có | Singleton, DontDestroyOnLoad, quản lý `currentDay` + `TimeOfDay` + `AdvanceTime()` |
| `UIManager` | ✅ Đã có | Singleton, DontDestroyOnLoad, HUD số ngày + sprite buổi |
| `SaveSystem` | ✅ Đã có | Singleton, DontDestroyOnLoad, lưu/load JSON vào `Assets/Save/` |
| `MenuHandler` | ✅ Đã có | Khởi tạo persistent managers, chuyển scene từ StartMenu |
| `NameInputHandler` | ✅ Đã có | Panel nhập tên player, validate, lưu PlayerPrefs |
| `MenuManager` | ❌ Chưa có | Xử lý Menu X in-game (Status/Save/Quit) |
| `CutsceneManager` | ❌ Chưa có | Trigger cutscene theo ID |
| `NPCSpawnManager` | ❌ Chưa có | Spawn NPC theo lịch ngày |
| `SceneTransitionManager` | ❌ Chưa có | Chuyển scene + fade |
| `EndingManager` | ❌ Chưa có | Kiểm tra điều kiện ending |

---

## ⚠️ Lưu Ý Quan Trọng Cho Copilot

1. **Unity 6** – dùng `Rigidbody2D.linearVelocity` thay vì `.velocity` (deprecated).
2. **Dialogue text có thể chứa tiếng lóng Gen Z, từ thô tục** – đây là nội dung story đã được duyệt, không phải lỗi.
3. **`{playerName}`** trong dialogue là placeholder, replace runtime bằng `PlayerPrefs.GetString("PlayerName")`.
4. **Nhóm Tứ Đại Thạch Hầu** (Hưng Lớn, Quân Shiba, Joli, Uyển Chi) – tương tác 1 NPC → cả 4 đều +25 thiện cảm.
5. **Contact 4 của Nhân Lê và Harry là chung** (cùng cutscene bida).
6. **Jessica có `maxAffinity = 50`**, không phải 100. Sau scene 2 của Jessica, điểm thiện cảm không tăng thêm và không hiển thị mốc tối đa.
7. **Scene 3 Tony Thắng + Minh Anh** chỉ trigger khi **cả hai** >= 50% thiện cảm.
8. **NPC Easter Egg** chỉ cần static sprite (1 frame), không cần walk/idle animation.
9. **Bad Ending Thông** kết thúc bằng chuỗi sự kiện tự động sau Contact 4.
10. **Chuyển scene** dùng `SceneManager.LoadScene("TenScene")` với tên cố định, kết hợp `PlayerPrefs` + `SpawnPoint` pattern.
11. **NPC affection mốc**: 25%, 50%, 75%, 100% – mỗi mốc trigger `UnityEvent` 1 lần duy nhất.
12. Ưu tiên sử dụng thao tác trên unity (các game object và component đính kèm script) thay vì code sinh ra UI chay, vì code sinh ra UI chay rất xấu và khó chỉnh sửa.
13. **Persistent Managers** (DayManager, UIManager, DialogueManager) được `MenuHandler.InitializePersistentManagers()` tạo từ prefab khi bắt đầu game mới, tồn tại xuyên suốt nhờ `DontDestroyOnLoad`.
14. **`PlayerMovement.canMove`** được `DialogueManager` set `false` khi mở bảng chọn option (tránh W/S di chuyển player), và set `true` khi chọn xong.

---

## 📅 Chu Kỳ Ngày (Daily Loop) – Triển Khai

```
[Đã có DayManager.cs]

[SÁNG] Thức dậy → Đến trường → Tương tác NPC / Học bài
   → Hoàn thành hành động → DayManager.AdvanceTime() → currentTime = Afternoon
   ↓
[CHIỀU] Tự học / Gặp NPC hành lang / Đi với Thông
   → Hoàn thành hành động → DayManager.AdvanceTime() → currentTime = Evening
   ↓
[TỐI] Về phòng → Ôn bài / Đi công viên / Ngủ sớm
   → Hoàn thành hành động → DayManager.AdvanceTime() → currentDay++, currentTime = Morning
```

### DayManager.cs – Logic thực tế đã triển khai
```csharp
public enum TimeOfDay { Morning, Afternoon, Evening }

// DayManager.AdvanceTime():
// Morning  → Afternoon
// Afternoon → Evening
// Evening  → currentDay++, Morning
// Sau mỗi lần: onTimeChanged?.Invoke() → UIManager cập nhật HUD tự động
```

### Ai gọi AdvanceTime()?
- **Hiện tại**: Chưa có script nào tự động gọi `DayManager.AdvanceTime()`. Cần gọi sau khi player hoàn thành hành động của buổi (ví dụ: sau `ApplyStatsAndProgress` trong `NPC_Interactor`, hoặc sau khi chọn hoạt động tại phòng trọ).
- **Thiết kế**: Mỗi buổi player chọn **1 hành động** → apply stats → gọi `AdvanceTime()` → UIManager tự động đổi sprite buổi + số ngày.

### Quy tắc reset mỗi ngày (Chưa triển khai)
- Khi `Evening → Morning` (ngày mới): cần reset `interactedToday` của tất cả NPC.
- Mỗi ngày **3 NPC vắng mặt** theo chu kỳ 4 ngày, vị trí spawn random từ Day 6+.

---

## 🗓️ UI Ngày Hiện Tại (Day Counter HUD) – Đã Triển Khai

- **Vị trí**: Góc **trái bên trên** màn hình, luôn hiển thị đè lên mọi scene.
- **Cấu trúc**: Số ngày (`dayCountText` – TextMeshPro) + Sprite buổi (`sessionImage` – Image)
- **Cập nhật**: Tự động refresh mỗi khi `DayManager.onTimeChanged` fire.
- **Implement**: `UIManager.cs` (Singleton, DontDestroyOnLoad) subscribe event từ `DayManager`.
- **Sprite buổi**: `morningSprite`, `afternoonSprite`, `eveningSprite` – gán trong Inspector.

```csharp
// UIManager.cs – cập nhật HUD (code thực tế)
public void UpdateDayUI()
{
    dayCountText.text = $"{DayManager.Instance.CurrentDay}";
    sessionImage.sprite = DayManager.Instance.CurrentTime switch
    {
        TimeOfDay.Morning   => morningSprite,
        TimeOfDay.Afternoon => afternoonSprite,
        TimeOfDay.Evening   => eveningSprite,
        _ => morningSprite
    };
}
```
---

## 🖥️ Giao Diện Menu (Nhấn X)

Menu được implement bằng **Canvas/Panel UI** tồn tại xuyên suốt tất cả scene (`DontDestroyOnLoad` hoặc scene riêng dạng Additive). Nhấn `X` ở bất kỳ scene nào đều mở được.

### Cấu trúc Menu
- **Điều khiển**: mũi tên `↑ ↓` để di chuyển, `Z` để chọn, `X` để quay lại / đóng menu.
- **3 lựa chọn chính:**

| Mục | Nội dung |
|---|---|
| **Status** | Điểm Học lực + Điểm Stress hiện tại của Player. Bên dưới là danh sách NPC gồm: tên, ảnh face (sprite), điểm thiện cảm với NPC đó. |
| **Save** | Lưu game ra file JSON vào folder `Save/`. |
| **Quit Game** | Thoát game (`Application.Quit()`). |

### Lưu ý implement
- Script quản lý menu: `MenuManager.cs` (Singleton, `DontDestroyOnLoad`).
- Khi menu mở → set `Time.timeScale = 0` hoặc disable `PlayerMovement`.
- Danh sách NPC trong Status đọc từ tất cả `NPCData` ScriptableObject đang active.

---

## 💬 Giao Diện Dialogue (Trò Chuyện NPC) – Đã Triển Khai

Dialogue UI là một Canvas/Panel hiển thị khi Player tương tác với NPC (nhấn `Z`). Quản lý bởi `DialogueManager.cs` (Singleton, DontDestroyOnLoad).

### Cấu trúc layout
```
┌─────────────────────────────────────────────────────┐
│ [Avatar]  │  Message Box (nền đen)                  │
│  NPC      │  Script trò chuyện hiển thị ở đây...    │
│  Face     │                                         │
│           │                                         │
│ [NameTag] │  [hintText: "Z: Tiep tuc | X: Huy"]    │
└─────────────────────────────────────────────────────┘

Khi có choices (bên trong MessageBox, thay thế messageText):
┌─────────────────────────────────────────────────────┐
│ [Avatar]  │  ┌─────────────────────────────────┐    │
│  NPC      │  │ [▶] Ư, đi thôi!                 │    │
│  Face     │  │ [ ] Thôi, mình bận rồi.         │    │
│           │  └─────────────────────────────────┘    │
│ [NameTag] │  [hintText: "W/S: Chon | Z: Xac nhan"] │
└─────────────────────────────────────────────────────┘
```

### Dialogue cơ bản
- **Message Box**: nền màu đen, text trắng, hiển thị nội dung dialogue từng dòng
- **Typing Effect**: text hiện từng ký tự (tốc độ `typingSpeed = 0.05f`), nhấn `Z` skip → hiện full text ngay
- **Avatar**: nằm bên **trái** message box, sprite face từ `NPCData.faceSprite`
- **Name Tag**: khung hiển thị tên NPC, nằm **dưới avatar**
- **hintText**: dòng cuối message box, tự động thay đổi theo trạng thái:
  - Dòng giữa → `"Z: Tiep tuc | X: Huy"` (gray)
  - Dòng cuối + có choices sau → `"Z: Tiep tuc | X: Huy"` (gray)
  - Dòng cuối + không có choices → `"Z: Dong | X: Huy"` (cyan)
- **Placeholder**: `{playerName}` và `{player}` → replace runtime bằng `PlayerPrefs.GetString("PlayerName")`

### Hệ thống Choices (Chọn Option)
- **choicePanel**: nằm **bên trong** MessageBox, ẩn messageText khi hiện choices
- **optionRows[]**: mỗi row chứa 1 `cursorImage` (sprite `Btn_Play`) + 1 `optionText` (TextMeshPro)
- **Cursor**: sprite `Btn_Play` (tại `Assets/Sprites/UI`), chỉ hiện ở row đang chọn
- **Điều khiển**: `W/S` hoặc `↑/↓` di chuyển cursor, `Z` xác nhận
- **Movement Lock**: khi `ShowChoices()` → set `PlayerMovement.canMove = false` (tránh W/S di chuyển player), khi `ConfirmChoice()` → set `canMove = true`
- **Callback**: `ConfirmChoice()` gọi `onChoiceSelected(selectedIndex)` → `NPC_Interactor.ApplyStatsAndProgress(slot, choiceIndex)`

### Cấu trúc Hierarchy trong Unity Editor
```
DialogueManager (Canvas, DontDestroyOnLoad)
└── DialoguePanel
    ├── AvatarImage (Image)
    ├── NameTagText (TextMeshPro)
    ├── MessageBox
    │   ├── MessageText (TextMeshPro)
    │   └── ChoicePanel (ẩn mặc định)
    │       ├── OptionRow_0
    │       │   ├── CursorImage_0 (Btn_Play sprite)
    │       │   └── OptionText_0 (TextMeshPro)
    │       └── OptionRow_1
    │           ├── CursorImage_1 (Btn_Play sprite)
    │           └── OptionText_1 (TextMeshPro)
    └── HintText (TextMeshPro)
```

---

## 🏠 Scene StartMenu (Start Menu) – Đã Triển Khai

Scene **`StartMenu`** (Build Settings index 0). Sử dụng 2 script: `MenuHandler.cs` + `NameInputHandler.cs`.

### Cấu trúc Scene
- **Background**: Sprite nhân vật nữ kéo vào Canvas làm background.
- **3 nút UI Button** (dùng uGUI + TextMeshPro để hỗ trợ tiếng Việt):

| Nút | Script xử lý | Hành động |
|---|---|---|
| **Chơi Mới** | `MenuHandler.StartNewGame()` | Gọi `NameInputHandler.OpenNameInputPanel()` → hiện panel nhập tên |
| **Chơi Tiếp** | `MenuHandler.ContinueGame()` | Check `SaveSystem.Instance.HasSaveFile()` → `LoadGame()` → load scene |
| **Thoát** | `MenuHandler.QuitGame()` | `Application.Quit()` (Editor: `EditorApplication.isPlaying = false`) |

### Nhập Tên Người Chơi (`NameInputHandler.cs`)
- **Chơi Mới** → `MenuHandler.StartNewGame()` → `NameInputHandler.OpenNameInputPanel()`
- Panel nhập tên: `TMP_InputField` + nút **Xác nhận** + nút **Hủy**
- Nút Hủy → ẩn panel nhập tên, hiện lại menu chính
- Nút Xác nhận hoặc nhấn Enter → `ConfirmPlayerName()`:
  1. Validate tên không rỗng (`string.IsNullOrWhiteSpace`)
  2. Lưu tên: `PlayerPrefs.SetString("PlayerName", playerName)` + `PlayerPrefs.Save()`
  3. Gọi `MenuHandler.InitializePersistentManagers()` → tạo DayManager, UIManager, DialogueManager từ prefab
  4. `SceneManager.LoadScene("Map00_BenXe")` (tên scene cố định)

### MenuHandler.InitializePersistentManagers()
- Kiểm tra `[Tên]Manager.Instance == null` trước khi Instantiate → tránh duplicate
- 3 prefab cần gán trong Inspector: `dayManagerPrefab`, `uiManagerPrefab`, `dialogueManagerPrefab`
- Sau Instantiate → `SetActive(true)` để đảm bảo `Awake()` chạy gán `Instance`
- Các manager tồn tại xuyên suốt nhờ `DontDestroyOnLoad` trong `Awake()` của từng script

### Chơi Tiếp (`MenuHandler.ContinueGame()`)
- Check `SaveSystem.Instance` != null và `HasSaveFile()` trước khi load
- Gọi `SaveSystem.Instance.LoadGame()` → `PlayerDataManager.Instance.LoadFromSaveData(data)`
- Load scene từ `SaveData.currentScene` (fallback: `"Scene_Map001_Home"`)
---

## 💾 Hệ Thống Save/Load – Đã Triển Khai

Lưu tiến trình ra file **JSON** trong folder `Assets/Save/` (runtime: `Application.dataPath/Save/`). Sử dụng 3 script: `SaveData.cs`, `SaveSystem.cs`, `PlayerDataManager.cs`.

### SaveData.cs – Dữ liệu lưu trữ (code thực tế)
```csharp
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

    // Thiện cảm từng NPC (13 NPC)
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

    // Constructor mặc định: money = 5_000_000, currentDay = 1, currentScene = "Scene_Map001_Home"
}
```

### SaveSystem.cs (Singleton, DontDestroyOnLoad) – API
| Method | Mô tả |
|---|---|
| `SaveGame()` | `PlayerDataManager.SaveToData(data)` → `JsonUtility.ToJson()` → ghi file |
| `LoadGame()` | Đọc file JSON → `JsonUtility.FromJson<SaveData>()` → `PlayerDataManager.LoadFromSaveData(data)` → return `bool` |
| `HasSaveFile()` | Kiểm tra file save tồn tại |
| `DeleteSaveFile()` | Xóa file save (Reset game) |
| `GetCurrentSaveData()` | Trả về `SaveData` đã load gần nhất |

### Luồng Save/Load
```
[SAVE] MenuManager nhấn Save
  → SaveSystem.Instance.SaveGame()
    → PlayerDataManager.Instance.SaveToData(data)  // copy stats + NPC affinity vào SaveData
    → data.currentScene = SceneManager.GetActiveScene().name
    → JsonUtility.ToJson(data) → File.WriteAllText("Assets/Save/savegame.json")

[LOAD] MenuHandler nhấn "Chơi Tiếp"
  → SaveSystem.Instance.LoadGame()
    → File.ReadAllText → JsonUtility.FromJson<SaveData>(json)
    → PlayerDataManager.Instance.LoadFromSaveData(data)  // ghi stats + NPC affinity vào memory
    → return true
  → MenuHandler đọc data.currentScene → SceneManager.LoadScene(scene)
```

### PlayerDataManager ↔ SaveData Bridge
- `SaveToData(SaveData)`: copy `playerName`, `academicScore`, `stress`, `money`, `currentDay`, `failedExams` + 13 `npcAffinity[key]` → 13 field `aff_*`
- `LoadFromSaveData(SaveData)`: ngược lại, ghi từ `SaveData` → fields + dictionary
- NPC affinity keys: `"Harry"`, `"HungBig"`, `"HungSmall"`, `"Jessica"`, `"Joli"`, `"KieuVy"`, `"MinhAnh"`, `"MrTam"`, `"NhanLe"`, `"QuanShiba"`, `"Thong"`, `"TonyThang"`, `"UyenChi"`

### Lưu ý
- File save lưu tại: `[ProjectRoot]/Assets/Save/savegame.json`
- Thêm `Assets/Save/` vào `.gitignore` để không commit file save lên repo.
- `SaveSystem` dùng `try/catch` để xử lý lỗi IO an toàn.

---

## 🔗 Kết Nối Cổng Teleport & Intro

### Intro Cutscene → Map
- Sau khi đoạn thoại Intro kết thúc, gọi: `SceneManager.LoadScene("Map00_BenXe")`

### Teleport Gate
- Tất cả cổng teleport dùng `OnTriggerEnter2D` (Box Collider 2D – **Is Trigger** = true).
- Trước khi `LoadScene`, lưu SpawnPoint ID qua `PlayerPrefs.SetString("SpawnPoint", id)`.
- `SpawnPoint.cs` tại scene đích đọc ID và teleport Player đến đúng vị trí.
- **Bắt buộc**: Thêm tất cả Scene vào **Build Settings** (File → Build Settings) theo đúng thứ tự.

### Thứ tự Scene khuyến nghị trong Build Settings

| Index | Scene |
|---|---|
| 0 | `StartMenu` |
| 1 | `Map00_BenXe` |
| 2 | `Map000_Strange_guy` |
| 3 | `Scene_Map001_Home` |
| 4+ | Các scene còn lại |

---

## 📦 Xuất File .exe (Build Game)

1. Vào **File → Build Settings**.
2. Chọn nền tảng **Windows, Mac, Linux Standalone**.
3. Đảm bảo tất cả Scene đã được kéo vào danh sách **Scenes In Build**.
4. Nhấn **Build** → chọn folder output.
5. File `.exe` và folder `_Data` cần nằm cùng thư mục để chạy được.

---

*File này phản ánh trạng thái thực tế của project tại thời điểm tạo. Cập nhật khi có thay đổi lớn về cấu trúc hoặc thêm Manager mới.*
