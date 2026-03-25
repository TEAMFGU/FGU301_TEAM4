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
│   ├── MenuHandler.cs             # Chuyển scene từ menu
│   ├── PlayerMovement.cs          # Di chuyển WASD, Animator Blend Tree
│   ├── SpawnPoint.cs              # Spawn player đúng vị trí khi chuyển scene
│   ├── TeleportPortal.cs          # Chọn điểm đến + chuyển scene
│   └── NPC/
│       ├── NPCData.cs             # ScriptableObject – dữ liệu NPC
│       └── NPC_Interactor.cs      # Hệ thống tương tác + cutscene mốc
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

### InteractionManager.cs
- Singleton (`Instance`)
- `Z` = tương tác / xác nhận
- `X` = hủy / mở menu
- Flag `isInteracting` để quản lý trạng thái

### NPCData.cs (ScriptableObject)
- `[CreateAssetMenu(menuName = "FPT Adventure/NPC Data")]`
- Fields: `npcName`, `description`, `initialAffection`, `routeType`, `level` (1-4), `affection` (0-100)
- Cutscene flags: `unlockedCutscene25`, `unlockedCutscene50`, `unlockedCutscene75`, `unlockedCutscene100`

### NPC_Interactor.cs
- Gắn vào mỗi NPC GameObject
- Mỗi lần `Interact()` → `affection += 25` (max 100)
- 4 mốc cutscene: 25%, 50%, 75%, 100% → trigger `UnityEvent`
- `DialogueSlot[]` hỗ trợ: npcText, playerChoices, affectionChange, stressChange, studyChange
- `currentLevel` 1→4, tăng mỗi lần tương tác

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

## 📊 Hệ Thống Chỉ Số (Stats System) – Thiết Kế

```csharp
[System.Serializable]
public class PlayerStats
{
    public string playerName;
    public int money = 5_000_000;           // VNĐ (trừ 200k xe ôm + 500k cọc nhà)
    public int academicScore = 0;           // Học lực: 0–100, cần >= 60 pass môn
    public int stress = 0;                  // Stress: 0–100, >= 80 warning
    public int currentDay = 1;
    public Dictionary<string, int> npcAffinity;
    public Dictionary<string, int> npcContactLevel;
    public Dictionary<string, bool> interactedToday;
    public RoomType chosenRoom;
}
```

### Bảng Thay Đổi Chỉ Số (Theo Buổi)

| Buổi | Hành động | Học lực | Stress |
|---|---|:---:|:---:|
| **SÁNG** | Học bài | +5 | +15 |
| **SÁNG** | Gặp NPC (Lớp) | 0 | -5 |
| **TRƯA** | Tự học | +5 | +10 |
| **TRƯA** | Gặp NPC (Hành lang) | 0 | -5 |
| **TRƯA** | Đi với Thông | -10 | -40 |
| **CHIỀU** | Ôn bài ở nhà | +10 | +20 |
| **CHIỀU** | Đi công viên | 0 | -30 |
| **CHIỀU** | Ngủ sớm | 0 | -50 |

### Sự kiện cố định

| Sự kiện | Học lực | Stress | Tiền (VNĐ) |
|---|:---:|:---:|:---:|
| Xe ôm – cutscene Map0 | — | — | -200.000 |
| Cọc nhà trọ – cutscene Map00 | — | — | -500.000 |

> **Khi NPC đã được tương tác trong ngày hoặc điểm thiện cảm đạt tối đa**, thoại chung của tất cả NPC là `[.............................]`

---

## 🏁 Hệ Thống Ending

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

## 🧠 Managers Cần Có (Đã có / Chưa có)

| Manager | Trạng thái | Chức năng |
|---|---|---|
| `InteractionManager` | ✅ Đã có | Singleton, xử lý Z/X input |
| `PlayerDataManager` | ❌ Chưa có | Lưu/đọc PlayerStats, persist qua scene |
| `DialogueManager` | ❌ Chưa có | Quản lý UI hộp thoại, avatar NPC, name tag |
| `MenuManager` | ❌ Chưa có | Singleton, DontDestroyOnLoad, xử lý Menu X (Status/Save/Quit) |
| `SaveSystem` | ❌ Chưa có | Singleton, DontDestroyOnLoad, lưu/load JSON vào `Assets/Save/` |
| `CutsceneManager` | ❌ Chưa có | Trigger cutscene theo ID |
| `DayManager` | ❌ Chưa có | Quản lý ngày, reset interactedToday |
| `NPCSpawnManager` | ❌ Chưa có | Spawn NPC theo lịch ngày |
| `SceneTransitionManager` | ❌ Chưa có (dùng TeleportPortal) | Chuyển scene + fade |
| `EndingManager` | ❌ Chưa có | Kiểm tra điều kiện ending |
| `UIManager` | ❌ Chưa có | HUD hiển thị stats, ngày, tiền |

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
10. **Chuyển scene** hiện dùng `PlayerPrefs` + `SpawnPoint` pattern, chưa có `DontDestroyOnLoad`.
11. **NPC affection mốc**: 25%, 50%, 75%, 100% – mỗi mốc trigger `UnityEvent` 1 lần duy nhất.

---

## 📅 Chu Kỳ Ngày (Daily Loop) – Thiết Kế

```
[SÁNG] Thức dậy → Đến trường → Tương tác NPC / Học bài
   ↓
[CHIỀU] Tự học / Gặp NPC hành lang / Đi với Thông
   ↓
[TỐI] Về phòng → Ôn bài / Đi công viên / Ngủ sớm → ⏭ Qua ngày mới
```

### 3 Buổi Mỗi Ngày
- Mỗi ngày gồm **3 buổi**: **Sáng → Chiều → Tối**.
- Mỗi khi player **hoàn thành hành động** của một buổi (tương tác NPC, học bài, chọn hoạt động,...) → **tự động chuyển sang buổi tiếp theo**.
- Hết buổi **Tối** → **sang Ngày mới** (currentDay += 1), reset `interactedToday`.
- UI ngày hiện tại cập nhật ngay khi `currentDay` thay đổi.

### Trạng Thái Buổi (TimeOfDay)
```csharp
public enum TimeOfDay { Morning, Afternoon, Evening }
```
- `DayManager` quản lý `currentDay` và `currentTime`.
- Khi `currentTime == Evening` và player kết thúc hành động → `currentDay++`, `currentTime = Morning`.

Mỗi ngày **3 NPC vắng mặt** theo chu kỳ 4 ngày, vị trí spawn random từ Day 6+.

---

## 🗓️ UI Ngày Hiện Tại (Day Counter HUD)

- **Vị trí**: Góc **trái bên trên** màn hình, luôn hiển thị đè lên mọi scene.
- **Nội dung**: Hiển thị ngày và buổi hiện tại. Ví dụ: `Ngày 1 – Sáng`, `Ngày 3 – Chiều`.
- **Cập nhật**: Tự động refresh mỗi khi `DayManager` thay đổi `currentDay` hoặc `currentTime`.
- **Implement**: Là một `Canvas` với `DontDestroyOnLoad`, chứa **TextMeshPro** text.
- **Script**: `UIManager.cs` lắng nghe sự kiện từ `DayManager` và cập nhật text.

```
┌─────────────────────────────────────────┐
│ [Ngày 1 – Sáng]          [Stats HUD...] │  ← Top-left
│                                         │
│           (Game Scene)                  │
└─────────────────────────────────────────┘
```

```csharp
// UIManager.cs – cập nhật text ngày
private void UpdateDayUI()
{
    string session = DayManager.Instance.CurrentTime switch
    {
        TimeOfDay.Morning   => "Sáng",
        TimeOfDay.Afternoon => "Chiều",
        TimeOfDay.Evening   => "Tối",
        _ => ""
    };
    dayText.text = $"Ngày {DayManager.Instance.CurrentDay} – {session}";
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

## 💬 Giao Diện Dialogue (Trò Chuyện NPC)

Dialogue UI là một Canvas/Panel hiển thị khi Player tương tác với NPC (nhấn `Z`).

### Cấu trúc layout
```
┌─────────────────────────────────────────────────────┐
│ [Avatar]  │  Message Box (nền đen)                  │
│  NPC      │  Script trò chuyện hiển thị ở đây...    │
│  Face     │                                         │
│           │                                         │
│ [NameTag] │                                         │
└─────────────────────────────────────────────────────┘
```

- **Message Box**: nền màu đen, text trắng, hiển thị nội dung dialogue từng dòng.
- **Avatar**: nằm bên **trái** message box, kéo sprite face của NPC đang trò chuyện vào.
- **Name Tag**: khung hiển thị tên NPC, nằm **dưới avatar**.
- **Chiều cao** của message box = chiều cao avatar + name tag.
- Sprite face NPC lấy từ field `faceSprite` trong `NPCData` ScriptableObject.
- Điều khiển: `Z` để tiếp tục / xác nhận lựa chọn, `X` để đóng (nếu cho phép).

---

## 🏠 Scene StartMenu (Start Menu)

Tạo một Scene mới tên là **`StartMenu`** (thêm vào Build Settings ở vị trí index 0).

### Cấu trúc Scene
- **Background**: Sprite nhân vật nữ kéo vào Canvas làm background.
- **3 nút UI Button** (dùng uGUI + TextMeshPro để hỗ trợ tiếng Việt):

| Nút | Hành động |
|---|---|
| **Chơi Mới** | Hiện khung nhập tên → lưu tên → Load scene `Map00_BenXe` |
| **Chơi Tiếp** | Gọi `SaveSystem.Instance.LoadGame()` |
| **Thoát** | `Application.Quit()` |

- **Yêu cầu**: Cài **uGUI** và **TextMeshPro** (Window → Package Manager) để nút hiển thị đúng và gõ được tiếng Việt.

### Nhập Tên Người Chơi (Khi nhấn "Chơi Mới")
- Nhấn **Chơi Mới** → hiện **Panel nhập tên** (InputField + nút Xác nhận).
- Player nhập tên → nhấn Xác nhận → lưu tên vào `PlayerPrefs.SetString("PlayerName", inputText)`.
- Sau đó gọi `SceneManager.LoadScene("Map00_BenXe")`.
- Tên này được dùng xuyên suốt game qua `PlayerPrefs.GetString("PlayerName")`.
- Placeholder trong dialogue dùng `{playerName}` → replace runtime bằng tên đã lưu.
- **Lưu ý**: Không cho phép tên rỗng – validate trước khi xác nhận.

```csharp
// Trong MenuHandler.cs
public void ConfirmPlayerName(string inputName)
{
    if (string.IsNullOrWhiteSpace(inputName)) return;
    PlayerPrefs.SetString("PlayerName", inputName.Trim());
    PlayerPrefs.Save();
    SceneManager.LoadScene("Map00_BenXe");
}
```

---

## 💾 Hệ Thống Save/Load (SaveSystem.cs)

Lưu tiến trình ra file **JSON** trong folder `Assets/Save/` (runtime: `Application.dataPath/Save/`).

### SaveData – Dữ liệu cần lưu
```csharp
[System.Serializable]
public class SaveData
{
    public string currentScene;
    public int academicScore;
    public int stress;
    public int money;
    public int currentDay;
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
}
```

### SaveSystem.cs
```csharp
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private const string SaveFolder = "Save";
    private const string SaveFile = "savegame.json";

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.currentScene = SceneManager.GetActiveScene().name;
        // Đọc từ PlayerDataManager.Instance hoặc NPCData assets
        string folderPath = Path.Combine(Application.dataPath, SaveFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(Path.Combine(folderPath, SaveFile), json);
        Debug.Log("Đã lưu game!");
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.dataPath, SaveFolder, SaveFile);
        if (!File.Exists(path)) { Debug.LogWarning("Không tìm thấy file save!"); return; }
        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        // Ghi lại vào PlayerDataManager và NPCData assets
        SceneManager.LoadScene(data.currentScene);
    }
}
```

- File save lưu tại: `[ProjectRoot]/Assets/Save/savegame.json`
- Thêm `Assets/Save/` vào `.gitignore` để không commit file save lên repo.

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
