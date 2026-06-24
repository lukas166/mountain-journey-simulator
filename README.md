# Mountain Journey Simulator

Mountain Journey Simulator adalah project simulasi perjalanan gunung berbasis Unity dengan dukungan VR/XR. Project ini menghadirkan pengalaman eksplorasi lingkungan pegunungan melalui scene interaktif, sistem quest, interaksi NPC, radial menu, audio, dan pengaturan kenyamanan pengguna.

## Deskripsi Project

Project ini dibuat sebagai simulator perjalanan di area pegunungan. Pengguna memulai pengalaman dari menu utama, lalu masuk ke scene VR untuk menjelajahi environment, membaca informasi quest, berinteraksi dengan NPC, dan menggunakan menu radial untuk mengakses fitur tertentu.

Pengalaman utama project berfokus pada:

- Eksplorasi area pegunungan dalam mode VR.
- Informasi quest berbasis halaman teks dan gambar.
- Interaksi area tertentu melalui trigger.
- Menu radial yang mengikuti arah pandang pengguna.
- Pengaturan audio dan tipe rotasi pemain.

## Fitur

- Main menu dengan halaman start, options, about, dan quit.
- Scene transition dengan efek fade.
- VR scene berbasis XR Interaction Toolkit.
- Quest window dengan navigasi halaman.
- Quest trigger saat pemain memasuki area tertentu.
- NPC window untuk menampilkan dialog atau informasi.
- NPC trigger untuk membuka informasi dan mengatur barrier.
- Radial menu berbasis input controller.
- Toggle audio global melalui radial menu.
- Pengaturan volume dari menu options.
- Pilihan tipe rotasi pemain antara snap turn dan continuous turn.
- Audio manager untuk mengatur pemutaran audio berdasarkan nama.

## Teknologi

- Unity `6000.3.11f1`
- XR Interaction Toolkit `3.3.1`
- OpenXR `1.16.1`
- XR Management `4.6.0`
- XR Hands `1.8.0`
- Input System `1.19.0`
- AR Foundation `6.3.4`
- Unity UI dan TextMeshPro
- Universal Render Pipeline dan High Definition Render Pipeline tersedia di package project

## Struktur Project

```text
Assets/
  Scenes/                 Scene utama project
  Scripts/                Script gameplay, UI, quest, NPC, audio, dan transisi scene
  Materials/              Material untuk environment dan UI
  Audio/                  Asset audio
  XRI/                    Konfigurasi XR Interaction Toolkit
  XR/                     Konfigurasi XR dan loader
  Samples/                Sample package Unity XR
  Flooded_Grounds/        Asset environment dan post-processing
  Forst/                  Asset vegetasi dan shader

Packages/
  manifest.json           Daftar dependency Unity Package Manager

ProjectSettings/
  ProjectVersion.txt      Versi Unity project
  EditorBuildSettings.asset
```

## Scene

Scene yang digunakan dalam Build Settings:

1. `Assets/Scenes/Menu Scene.unity`
2. `Assets/Scenes/VRScene.unity`

`Menu Scene` digunakan sebagai halaman awal project. Saat tombol Start ditekan, project memuat `VRScene` melalui `SceneTransitionManager`.

## Alur Aplikasi

1. Pengguna membuka project dari `Menu Scene`.
2. Menu utama menampilkan pilihan Start, Options, About, dan Quit.
3. Tombol Start menjalankan transisi menuju `VRScene`.
4. Pengguna menjelajahi area pegunungan dalam scene VR.
5. Saat masuk ke area tertentu, trigger dapat membuka quest atau informasi NPC.
6. Quest dan NPC ditampilkan dalam window berbasis halaman.
7. Radial menu dapat digunakan untuk memilih fitur cepat dan mengatur audio.
8. Options menu dapat digunakan untuk mengatur volume dan tipe rotasi.

## Script Utama

- `Assets/Scripts/GameStartMenu.cs`: mengatur navigasi menu utama.
- `Assets/Scripts/SceneTransitionManager.cs`: mengatur perpindahan scene.
- `Assets/Scripts/FadeScreen.cs`: mengatur efek fade screen.
- `Assets/Scripts/QuestManager.cs`: mengatur tampilan dan navigasi quest.
- `Assets/Scripts/QuestZoneTrigger.cs`: memunculkan quest saat pemain masuk trigger.
- `Assets/Scripts/NPCWindowManager.cs`: mengatur tampilan informasi NPC.
- `Assets/Scripts/NPCZoneTrigger.cs`: memicu informasi NPC dan perubahan barrier.
- `Assets/RadialSelection.cs`: mengatur radial menu.
- `Assets/Scripts/SetOptionFromUI.cs`: mengatur volume dan preferensi rotasi dari UI.
- `Assets/Scripts/SetTurnTypeFromPlayerPref.cs`: menerapkan snap turn atau continuous turn.
- `Assets/Scripts/AudioManager.cs`: mengatur audio project.

## Cara Menjalankan

1. Clone repository:

   ```bash
   git clone <repository-url>
   cd mountain-journey-simulator
   ```

2. Buka Unity Hub.

3. Pilih `Add project from disk`.

4. Arahkan ke folder repository ini.

5. Buka project menggunakan Unity `6000.3.11f1`.

6. Tunggu Unity menyelesaikan proses import asset dan restore package.

7. Buka scene berikut:

   ```text
   Assets/Scenes/Menu Scene.unity
   ```

8. Tekan tombol Play di Unity Editor.

## Konfigurasi XR

Project ini menggunakan XR Management dan OpenXR. Jika ingin menjalankan project dengan perangkat VR:

1. Buka `Edit > Project Settings > XR Plug-in Management`.
2. Pastikan OpenXR aktif pada platform target.
3. Buka `Edit > Project Settings > OpenXR`.
4. Pastikan interaction profile sesuai dengan controller atau perangkat yang digunakan.
5. Jalankan project melalui Unity Editor atau build ke perangkat target.

## Build Project

Untuk membuat build:

1. Buka `File > Build Settings` atau `File > Build Profiles`.
2. Pilih target platform.
3. Pastikan scene berikut masuk ke daftar build:

   ```text
   Assets/Scenes/Menu Scene.unity
   Assets/Scenes/VRScene.unity
   ```

4. Pastikan konfigurasi XR sesuai target platform.
5. Pilih `Build` atau `Build And Run`.
