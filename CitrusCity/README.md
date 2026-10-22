# Citrus City — Unity 6 Prototype
## Playable Demo: Magnetic Tap Puzzle & Mediterranean Town

Tasarım dokümanındaki (GDD) Akdeniz teması, Magnetic Tap çekirdek mekaniği, kasaba restorasyonu ve tüm ekran konseptlerine uygun olarak hazırlanmış Unity 6 prototipi.

---

## 🚀 Hızlı Başlangıç (Quick Start)

### 1. Gereksinimler
- Unity Hub (En güncel)
- Unity Editor **6000.0.x LTS** (Unity 6 LTS)
- iOS için Xcode 15+, Android için Android SDK API 34

### 2. Projeyi Açma
1. Unity Hub → **Add** → **Add project from disk**
2. `citrus_city/CitrusCity/` klasörünü seçin → **Open**
3. Editör açıldığında paketlerin yüklenmesini bekleyin.

---

## 🎬 PDF Tasarımına Uygun Sahneleri Oluşturma (Tek Tıkla Kurulum)

Projede `SceneBootstrap` editör aracı bulunmaktadır. Sahneleri oluşturmak için:

### 1. MainMenu (Ana Ekran & Kasaba Manzarası — PDF Sayfa 6)
1. Unity'de: `File → New Scene → Empty` → `Assets/Scenes/MainMenu.unity` olarak kaydedin.
2. Üst menü: **`CitrusCity → Setup → Create Complete MainMenu (PDF Design)`**
3. Sahneyi kaydedin (`Cmd + S`).
   - *İçerik*: Limonata Kafe & Akdeniz kasabası arka planı, penceredeki kadın kafe sahibi avatarı, Can/Altın/Limon barları, Solar Streak rozeti (8/8 Aktif), Etkinlik Sandığı, Limonata Festivali, Web Shop (%10 Bonus), Havuz Onarım pini ve `▶ BÖLÜM 47 (ZOR)` butonu.

### 2. GameScene (Oyun Ekranı — PDF Sayfa 8)
1. Unity'de: `File → New Scene → Empty` → `Assets/Scenes/GameScene.unity` olarak kaydedin.
2. Üst menü: **`CitrusCity → Setup → Create Complete GameScene (PDF Design)`**
3. Sahneyi kaydedin (`Cmd + S`).
   - *İçerik*: Teras manzarası arka planı, ahşap tepsi tahtası, hücre zemin karoları, üst HUD (Avatar, Hedefler, Hamle sayacı), **Aurora Festivali Barı** (Sunlight Surge şarj barı), alt **Booster Bar** (Çekiç, Sopa, Sepet, Mikser), "Neredeyse Bitıyordu!" popup'ı ve Zafer paneli.

### 3. TownMapScene (Kasaba Haritası & Hikâye — PDF Sayfa 5)
1. Unity'de: `File → New Scene → Empty` → `Assets/Scenes/TownMapScene.unity` olarak kaydedin.
2. Üst menü: **`CitrusCity → Setup → Create TownMap Scene (PDF Design)`**
3. Sahneyi kaydedin (`Cmd + S`).
   - *İçerik*: "Şehir neden terk edildi?" 6 bölgeli ilerleme haritası (Limonata Kafe, Marina, Zeytinlik, Çarşı, Fener, Eski Konak) ve karakter diyalog kutusu.

---

## 📦 Projeye Dahil Edilen Asset'ler & Modeller

1. **Meyve & Engel Sprite'ları** (`Assets/Sprites/Fruits/` & `Assets/Sprites/Obstacles/`):
   - `Fruit_Orange.png`, `Fruit_Lemon.png`, `Fruit_Lime.png`, `Fruit_Cherry.png`, `Fruit_Grape.png`, `Fruit_Strawberry.png`
   - `Obstacle_Crate.png` (Tahta sandık engeli)
2. **Booster Sprite'ları** (`Assets/Sprites/Boosters/`):
   - `Booster_Rocket.png`, `Booster_Rocket_H.png`, `Booster_Rocket_V.png`
   - `Booster_Bomb.png`, `Booster_Vortex.png`
   - Seviye içi güçlendiriciler: `InGame_Booster_Hammer.png`, `InGame_Booster_Bat.png`, `InGame_Booster_Basket.png`, `InGame_Booster_Mixer.png`
3. **Arka Planlar & UI** (`Assets/Sprites/Backgrounds/` & `Assets/Sprites/UI/`):
   - `Bg_Town_MainMenu.png`, `Bg_Game_Terrace.png`, `Bg_Town_Map.png`, `Bg_Title_Banner.png`
   - `UI_Avatar_Window.png`, `Event_Solar_Streak.png`, `Event_Festival.png`, `Event_Chest.png`, `Event_Web_Shop.png`, `Event_Magic_Wand.png`
   - `Board_Tile.png` (Hücre zemin karoları)
4. **3D Mesh Modelleri** (`Assets/Models/`):
   - `Orange.obj`, `Lemon.obj`, `Lime.obj`, `Cherry.obj`, `Grape.obj`, `Bomb.obj`, `Rocket.obj`
5. **Hazır Seviyeler & Veriler** (`Assets/Resources/FruitData/` & `Assets/Resources/Levels/`):
   - `Level_01.asset`, `Level_02.asset`, `Level_03.asset` (Objective ve hamle sayıları önceden ayarlı).
