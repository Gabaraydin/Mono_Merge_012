# MonoMerge — Sahne & Prefab Kurulum Rehberi

Bu doküman, yazılmış olan 27 script dosyasını gerçek bir Unity sahnesine bağlamak için
gereken tüm Editor adımlarını sırayla anlatır. Kod tarafı bitti; geriye kalan her şey
GameObject oluşturmak, component eklemek ve Inspector'da referans sürüklemek.

**Önemli mimari not:** Tek sahne yeterli. `MainMenuController.HandlePlayClicked()` bir
sahne yüklemesi (SceneManager.LoadScene) yapmıyor, sadece menü panelini kapatıp oyunu
başlatıyor. Yani "MainMenu" ayrı bir `.unity` dosyası değil, aynı sahnedeki bir UI
panelidir. Tek sahnenin adı `Game.unity` olsun.

Sırayı takip et — her adım bir sonrakinin üzerine kuruluyor.

---

## 0. Hazırlık

- Unity Hub'dan projeyi aç (`Add project from disk` → `C:\Users\ASUS\Desktop\mobiloyun`).
- Package Manager'ın paketleri indirmesini bekle (ilk açılışta biraz sürebilir).
- `File > New Scene` → 2D template → `Assets/Scenes/Game.unity` olarak kaydet
  (mevcut boş `.gitkeep` dosyasının yanına).

---

## 1. Tile Tier Database asset'i oluştur

`Tiles/TileTierDatabase.cs` bir ScriptableObject — sahneye değil, Assets içine bir
"veri dosyası" olarak ekleniyor.

1. `Assets/Resources` klasörüne sağ tıkla → `Create > MonoMerge > Tile Tier Database`.
2. Adını `TileTierDatabase` bırak.
3. Inspector'da `Tiers` dizisini 6 elemana çıkar (Size: 6) ve şöyle doldur (koyulaşan
   gri tonları, GDD'nin "siyah/beyaz/nötr" yönergesine uygun):

   | Tier | Label | Color (RGB) |
   |---|---|---|
   | 1 | "1" | 200, 200, 200 |
   | 2 | "2" | 170, 170, 170 |
   | 3 | "3" | 140, 140, 140 |
   | 4 | "4" | 100, 100, 100 |
   | 5 | "5" | 60, 60, 60 |
   | 6 | "6" | 0, 0, 0 |

4. `Spawnable Tier Count` = **3** (GDD: tepsiye sadece 1/2/3 taşları düşer, 4+ sadece
   birleşerek ortaya çıkar).

---

## 2. Tile prefabı

1. Hierarchy'de sağ tık → `2D Object > Sprite` → adını `Tile` yap.
2. `Sprite Renderer` bileşeninde `Sprite` alanına `Assets/Sprites/tile_background.png`
   sürükle.
3. `Add Component > Box Collider 2D` (Tile.cs `[RequireComponent]` ile zaten istiyor,
   ama Unity otomatik eklemezse elle ekle). `Is Trigger` işaretli olmasın.
4. `Add Component > Tile` (script) — `MonoMerge.Tiles.Tile`.
5. Çocuk obje ekle: `Tile` üzerine sağ tık → `3D Object > Text - TextMeshPro` →
   adını `Label` yap, pozisyonunu (0,0,-0.1) yap (sprite'ın hafif önünde dursun).
   İlk açılışta TMP Essentials import etmeni isteyebilir — kabul et.
6. `Label`in `TextMeshPro` bileşeninde: Alignment = Center/Middle, Font Size uygun
   bir değer (örn. 8), rengi başlangıçta siyah kalabilir (Tile.cs zaten `SetTier`
   çağrılınca `spriteRenderer.color`'ı değiştiriyor, label rengini değiştirmiyor —
   istersen `Tile.cs`'e daha sonra label rengini de tier'a göre ayarlayan bir satır
   ekleyebiliriz).
7. `Tile` objesini seçip Inspector'ın üstündeki `Tile` script alanındaki `Label`
   referansına, az önce oluşturduğun `Label` child'ını sürükle.
8. `Layer` dropdown'ından `Add Layer...` ile yeni bir layer oluştur: **`Tile`**.
   Sonra `Tile` prefab objesinin Layer'ını `Tile` yap (bu, DragDropController'ın
   `tileLayerMask` ile sadece taşları raycast etmesini sağlayacak).
9. `Tile` objesini `Assets/Prefabs/Tile.prefab` olarak sürükle (prefab oluşturur),
   sonra sahneden sil (prefab yeterli, sahnede örneğine gerek yok).

---

## 3. Merge particle effect prefabı

1. Hierarchy'de sağ tık → `Effects > Particle System` → adını `MergeParticleEffect`
   yap.
2. Particle System ayarları (Inspector'da ilgili sekmeler):
   - `Start Lifetime`: 0.4
   - `Start Speed`: 2–3
   - `Start Size`: 0.05–0.1
   - `Start Color`: koyu gri/siyah (GDD: "ufak siyah noktaların saçılması")
   - `Emission > Rate over Time`: 0, `Bursts`: 1 burst, Count ~15
   - `Shape`: Cone veya Circle
   - `Renderer > Render Mode`: Billboard, `Material`'a `Assets/Sprites/particle_dot.png`
     kullanan bir "Sprites-Default" tabanlı materyal ata (Material oluştur:
     `Assets/Sprites/particle_dot.png` seçip Inspector'da Texture Type = **Sprite (2D
     and UI)** yap, sonra sağ tık → `Create > Material`, Shader = `Sprites/Default`,
     `Texture` alanına particle_dot.png'yi sürükle).
   - `Looping` kapalı olsun (Play One Shot mantığına uygun).
3. `Add Component > Merge Particle Effect` (script).
4. `Assets/Prefabs/MergeParticleEffect.prefab` olarak sürükle, sahneden sil.

---

## 4. Ana sahne iskeleti

Hierarchy'de şu boş GameObject'leri oluştur (`Create Empty`), her biri ayrı satır:

- `GridManager` — pozisyon (0,0,0)
- `TileSpawner`
- `MergeManager`
- `ScoreManager`
- `SaveManager`
- `UndoManager`
- `AudioManager`
- `GameManager`
- `DragDropController`
- `AdsManager`
- `BannerAdController`
- `InterstitialAdController`
- `RewardedAdController`

Her birine ilgili script'i `Add Component` ile ekle (isim = script adı, örn.
`GridManager` objesine `GridManager.cs` ekle). Bunların çoğunun component'i tek
başına yeterli, referansları adım 5'te bağlayacağız.

**Grid pozisyon matematiği** (GridManager varsayılanları: `cellSize = 1.2`,
`originWorldPosition = (0,0)`): hücre `(x, y)` dünya konumu `(x * 1.2, y * 1.2)`.
5x5 ızgara world-space'te yaklaşık `(0,0)` ile `(4.8, 4.8)` arasında yer alır.

**Tray slot'ları** oluştur — 3 boş GameObject:

- `TraySlot0` → pozisyon (0.0, -1.8, 0)
- `TraySlot1` → pozisyon (1.2, -1.8, 0)
- `TraySlot2` → pozisyon (2.4, -1.8, 0)

(Izgaranın altında, ekranın alt kısmında dursunlar — GDD: "Ekranın alt kısmında...
rastgele 2 veya 3 şekil verilir".)

**Kamera:** `Main Camera`'yı seç, `Add Component > Camera Shake`. Projection =
Orthographic, `Size` değerini ızgara + tepsi tamamı görünecek şekilde ayarla (örn.
4–5), pozisyonunu ızgaranın ortasına bakacak şekilde ayarla (örn. (2.4, 1.5, -10)).

**Izgara arka planı (görsel, opsiyonel ama önerilir):** `cell_background.png`'yi
sahneye sürükleyip 5x5 hücreyi kaplayacak tek bir büyük Sprite olarak ölçekle
(basit ve yeterli — 25 ayrı sprite yerine).

---

## 5. Referansları bağlama (Inspector)

Aşağıdaki her satır "bu objeyi seç → bu alana şunu sürükle" demek:

| Obje | Script alanı | Sürüklenecek şey |
|---|---|---|
| `TileSpawner` | Tile Prefab | `Assets/Prefabs/Tile.prefab` |
| `TileSpawner` | Tier Database | `TileTierDatabase` asset |
| `TileSpawner` | Tray Slots (size 3) | `TraySlot0`, `TraySlot1`, `TraySlot2` |
| `MergeManager` | Tier Database | `TileTierDatabase` asset |
| `MergeManager` | Merge Particle Prefab | `Assets/Prefabs/MergeParticleEffect.prefab` |
| `UndoManager` | Spawner | `TileSpawner` objesi |
| `GameManager` | Spawner | `TileSpawner` objesi |
| `DragDropController` | Main Camera | `Main Camera` |
| `DragDropController` | Tile Layer Mask | sadece `Tile` layer'ını seç |
| `DragDropController` | Spawner | `TileSpawner` objesi |
| `RewardedAdController` | Game Over Panel | (adım 6'da oluşturulacak `GameOverPanel`) |

`AudioManager`'ın `Place Clip` / `Merge Clip` / `Game Over Clip` alanları şimdilik
boş kalabilir — `AudioManager.cs` null clip'i güvenle atlıyor (sessiz çalışır, hata
vermez). Kendi ses dosyalarını eklediğinde buraya sürüklersin.

---

## 6. UI Canvas kurulumu

1. Hierarchy → sağ tık → `UI > Canvas` (otomatik bir `EventSystem` de gelir).
   `Canvas Scaler`'ı `Scale With Screen Size`, referans çözünürlük 1080x1920 yap
   (dikey mobil oyun).

2. **HUD** (Canvas altında boş obje `HUD`):
   - Çocuk: `TextMeshPro - Text (UI)` → adı `ScoreLabel`, ekranın üst ortasına
     hizala, büyük/kalın font (GDD: örnek arayüzdeki gibi sade, tek sayı).
   - `HUD` objesine `Add Component > HUD Controller`, `Score Label` alanına
     `ScoreLabel`'ı sürükle.

3. **MainMenuPanel** (Canvas altında, tüm ekranı kaplayan boş obje):
   - Çocuklar: başlık metni "MONOMERGE", `HighScoreLabel` (TMP text), `PlayButton`
     (UI Button), `SettingsButton` (UI Button).
   - `MainMenuPanel` objesine `Add Component > Main Menu Controller`, alanları
     sürükle (`High Score Label`, `Play Button`, `Settings Button`).

4. **SettingsPanel** (MainMenuPanel altında ya da Canvas altında ayrı, başlangıçta
   **inactive**):
   - Çocuklar: `MuteToggle` (UI Toggle), `CloseButton` (UI Button).
   - `Add Component > Settings Panel`, `Root` alanına kendi objesini (`SettingsPanel`)
     sürükle, `Mute Toggle` ve `Close Button` alanlarını bağla.
   - `MainMenuPanel`'in `Settings Panel Root` alanına bu `SettingsPanel` objesini
     sürükle.

5. **GameOverPanel** (Canvas altında, başlangıçta aktif kalabilir — script Awake'te
   kendini `SetActive(false)` yapıyor):
   - Çocuklar: `FinalScoreLabel`, `HighScoreLabel` (TMP text'ler), `RestartButton`,
     `WatchAdToContinueButton` (UI Button'lar).
   - `Add Component > Game Over Panel`, tüm alanları sürükle (`Root` = kendisi).
   - Şimdi adım 5'teki `RewardedAdController.Game Over Panel` alanına bu objeyi
     sürükleyebilirsin (geri dönüp bağla).

---

## 7. Test et

1. Play tuşuna bas.
2. `MainMenuPanel` görünmeli, `GameOverPanel` görünmemeli (Awake'te kapanıyor).
3. Play butonuna tıkla → panel kapanmalı, tepside 2-3 taş belirmeli.
4. Bir taşı sürükleyip boş bir hücreye bırak → hücreye "lerp" ile oturmalı, tepside
   eksilen taşın yerine (tepsi boşaldıysa) yeni taşlar gelmeli.
5. Aynı seviyeden 3 taşı yan yana/üst üste getir → birleşip üst tier'a dönüşmeli,
   parçacık efekti + kamera sarsıntısı tetiklenmeli, skor artmalı.
6. Izgarayı doldurup Game Over'a getir → `GameOverPanel` açılmalı, skor/yüksek skor
   görünmeli.

Bir adımda beklenmedik davranış görürsen (örn. taş sürüklenmiyor, birleşme
tetiklenmiyor) Console penceresindeki hata/uyarıları bana ilet, birlikte bakarız.

---

## 8. Bu rehberin kapsamadığı, hâlâ senin yapman gereken şeyler

- Gerçek ses dosyaları (place/merge/game-over click sesleri) — `AudioManager`
  alanlarına sürüklemen yeterli.
- Google Mobile Ads SDK kurulumu — önceki mesajlarda verdiğim 4 adım
  (`Ads/AdsManager.cs` sınıf yorumunda da yazılı).
- Gerçek mağaza ikonları, Player Settings (bundle ID, min OS sürümü vb.).
