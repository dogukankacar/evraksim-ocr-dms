<p align="center">
  <h1 align="center">Evraksim OCR DMS</h1>
  <p align="center">
    <strong>Akıllı ve OCR Destekli Modern Evrak Yönetim Sistemi</strong>
    <br/>
    <em>Next.js, ASP.NET Core, PostgreSQL, Docker, Tesseract OCR</em>
  </p>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Frontend-Next.js%2014-black?style=flat-square&logo=next.js" alt="Next.js" />
  <img src="https://img.shields.io/badge/Backend-ASP.NET%20Core%20API-blue?style=flat-square&logo=dotnet" alt=".NET" />
  <img src="https://img.shields.io/badge/Database-PostgreSQL-336791?style=flat-square&logo=postgresql" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/OCR-Tesseract-success?style=flat-square" alt="Tesseract OCR" />
  <img src="https://img.shields.io/badge/Docker-Ready-2496ED?style=flat-square&logo=docker" alt="Docker" />
</p>

## 🚀 Proje Hakkında

**Evraksim / Smart DMS**, kurumların veya bireylerin kağıt bazlı belgelerini ve dijital dökümanlarını merkezi bir mimaride saklamalarını, tam metin (full-text) destekli aramalar yapmalarını ve versiyon takiplerini gerçekleştirmelerini sağlayan **yapay zeka (OCR) entegrasyonlu** son teknoloji bir Doküman Yönetim Platformudur.

Sıradan dosya saklama araçlarının aksine bu proje, yüklenen dijital belgeleri (Resim ve PDF'ler) **Tesseract OCR (Optik Karakter Tanıma)** mimarisiyle tarayarak, içerisindeki tüm metinleri otomatik olarak bilgisayar ortamında aranabilir verilere çevirir.

## 🌟 Öne Çıkan Özellikler

- **🔍 OCR Destekli İçerik Okuma (Optical Character Recognition):** Yüklediğiniz dokümanlardaki veriler taranır, resim veya taranmış PDF olsalar dahi içlerindeki yazılar çıkartılıp sistemde saniyeler içinde aranabilir (Full-Text Search) kelimelere dönüştürülür.
- **🔄 Versiyon Kontrol Sistemi:** Birebir aynı belge güncellendiğinde, eski belgeleriniz üzerine yazılmaz. GitHub mantığında, her dokümanın `.V1, .V2` olarak geriye dönük versiyon geçmişi saklanır ve istenilen eski sürüm anında geri yüklenebilir/indirilebilir.
- **🔐 Gelişmiş Roller ve JWT Authenticator:** Administrator ve Standart Kullanıcı olarak departman bazlı veri ayırımı. (Identity & JWT Token mimarisi). Sadece Adminler işlem denetimlerini görebilir.
- **🛡️ Aksiyon Loglama (Audit Trails):** Sistem üzerinde hangi kullanıcının, saat kaçta, hangi evraka (yükleme, indirme, güncelleme, silme) eylemi yaptığı 7/24 veritabanına kaydedilir.
- **🐳 Tamamen Dockerize Edilmiş Yapı:** Zero-config sayesinde, hiçbir şey kurmadan tek bir Docker komutu ile bütün Micro-servisler (Database, Backend, Frontend) birbiri ile uyumlu ayağa kalkar.

## 🛠️ Teknoloji Yığını (Tech Stack)

| Katman | Teknoloji | Detay |
|--------|-----------|-------|
| **Backend API** | ASP.NET Core (Web API) | RESTful mimari yapısı, temiz kod standartları. |
| **Frontend** | Next.js + React.js + Tailwind CSS | Turbopack destekli Next.js 14, modern UI bileşenleri. |
| **Veritabanı** | PostgreSQL + EF Core | ORM ile Code-First mimarisi. |
| **OCR Motoru** | Tesseract (C# Wrapper) | PDF ve Görüntü (JPEG/PNG) metin tanıma işlemi. |
| **Canlı Ortam** | Docker & Docker Compose | Containerization ve Ağ optimizasyonu. |

## 📦 Kurulum ve Çalıştırma

Projenin kurulumu, Docker sayesinde tamamen otomatiktir. Bilgisayarınızda (veya sunucunuzda) [Docker](https://www.docker.com/) uygulamasının açık olması yeterlidir.

### Seçenek 1: Docker ile Hızlı Kurulum

1. Depoyu yerelinize klonlayın:
   ```bash
   git clone https://github.com/dogukankacar/evraksim-ocr-dms.git
   cd evraksim-ocr-dms
   ```

2. Docker üzerinden ana dizindeyken tek komutla tüm projeyi derleyip çalıştırın:
   ```bash
   docker-compose up --build -d
   ```

3. **Uygulamanız Yayında!** 🚀
   - **Kullanıcı Arayüzü (Frontend):** `http://localhost:3000`
   - **Backend API & Swagger Test Ekranı:** `http://localhost:5000/swagger`

*(Sistemi durdurmak için `docker-compose down` yazabilirsiniz.)*

---

## 🔑 Varsayılan (Test) Hesapları

Veritabanı ayağa kalktığında ilk testleri uygulayabilmeniz için Admin hesabı sistem tarafından otomatik olarak (Seed Data) oluşturulur.

* **Yetki:** Sistem Yöneticisi (Admin)
* **E-Posta:** `admin@dms.com`
* **Şifre:** `Admin123!`

*(Standart User testi yapmak için anasayfadan kendinize "Kayıt Ol" sekmesinden yeni bir hesap açabilirsiniz.)*

## 📁 Proje Klasör Mimarisi

```
evraksim-ocr-dms/
├── backend/                       # ASP.NET Core REST API
│   ├── DMS.API/                   # API Controller katmanı ve Startup ayarları
│   ├── DMS.Application/           # İş kuralları, servis katmanları, OCR entegrasyonu
│   ├── DMS.Core/                  # Temel modeller (Entity), Enum'lar ve İletişim sözleşmeleri
│   └── DMS.Infrastructure/        # EF Core PostgreSQL Data Context ve Loglama
├── frontend/                      # Web Client
│   ├── src/app/                   # Next.js App Router (Sayfalar)
│   ├── src/components/            # UI Modülleri (Navbar, ProtectedRoute vb.)
│   └── src/lib/                   # JWT Auth Helper ve Axios Service
├── docker-compose.yml             # Container Orchestration
└── README.md
```

## 🤝 Katkıda Bulunma (Contributing)
Proje açık kaynak kodludur, her türlü iyileştirmeye ve PR (Pull-Request) gönderimine açıktır. Özellikle OCR yeteneklerini geliştirme konusunda gelebilecek entegrasyonları destekliyoruz.

## 📄 Lisans
Bu proje geliştirilmeye açık ve öğrenmeye hazır açık kaynaklı bir referans depodur. İstediğiniz gibi kodları alıp kişisel/kurumsal ihtiyaçlarınızda kullanabilirsiniz.
