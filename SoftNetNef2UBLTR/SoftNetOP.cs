using SoftNetNef2UBLTR.SoftNetWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftNetNef2UBLTR
{
    public static class SoftNetOP
    {
        public static NetleBelge GetEarsiv() 
        {
            var nef = new NetleEFatura();

            var uniqueId = Guid.NewGuid();

            nef.GUID = uniqueId.ToString();// ETTN

            nef.Tedarikci = new Tedarikci()
            {
                FirmaAdi = "Tedarikci Firma LTD",
                VergiNoTCKimlikNo = "10203040506",
                Il = "İzmir",
                IlceSemt = "Konak",
                Ulke = "Türkiye",
                PostaKodu = "35000",
                VergiDairesi = "KONAK",
                Sokak = "123 Sokak No 35 ",
            };

            nef.Musteri = new Musteri()
            {
                FirmaAdi = "Muhtelif Müşteri",
                VergiNoTCKimlikNo = "11111111111",
                Il = "İzmir",
                IlceSemt = "Konak",
                Ulke = "TÜRKİYE",
                PostaKodu = "35000",
                VergiDairesi = "KONAK",
                Sokak = "Akdeniz Mah. Akdeniz Cad. No : 3/8",
            };

            nef.DuzenlenmeTarihi = DateTime.Now;
            nef.No = "SNA2024000000001";

            nef.DovizTipi = "TRY";
            nef.Tip = NetleEFaturaType.SATIS;
            nef.Senaryo = NetleEFaturaSenaryoType.TEMELFATURA;
            nef.KaynakDokumanTuru = KaynakDokumanTuru.EARSIV;

            nef.FaturaKalemleri = new FaturaKalemi[]
            {
                new FaturaKalemi()
                {
                    Miktar = 2,
                    StokAdi = "Hizmet Bedeli",
                    Birim = "NIU",
                    BirimFiyat = 10,
                    ToplamTutar = 20,
                    DovizTipi = "TRY",
                    Aciklama = "Yazılım Geliştirme Hizmeti",
                    Vergiler = new Vergi[]
                    {
                        new Vergi()
                        {
                            Matrah = 20,
                            Oran = 20,
                            Tur = TaxCodeContentType.Item0015,
                            Tutar = 4,
                        }
                    }
                },
                new FaturaKalemi()
                {
                    Miktar = 1,
                    StokAdi = "Hizmet Bedeli 2",
                    Birim = "NIU",
                    BirimFiyat = 10,
                    ToplamTutar = 10,
                    DovizTipi = "TRY",
                    Aciklama = "Yazılım Geliştirme Hizmeti",
                    Vergiler = new Vergi[]
                    {
                        new Vergi()
                        {
                            Matrah = 10,
                            Oran = 10,
                            Tur = TaxCodeContentType.Item0015,
                            Tutar = 1,
                        }
                    }
                }
            };

            nef.Vergiler = new Vergi[]
            {
                new Vergi()
                {
                    Matrah = 20,
                    Oran = 20,
                    Tur = TaxCodeContentType.Item0015,
                    Tutar = 2,
                },
                new Vergi()
                {
                    Matrah = 10,
                    Oran = 10,
                    Tur = TaxCodeContentType.Item0015,
                    Tutar = 1,
                }
            };

            nef.ToplamTutar = 23;
            nef.KDVMatrahi = 20;
            nef.KDVTutari = 3;
            nef.OdenecekToplamTutar = 23;

            return nef;
        }
    }
}
