using SoftNetNef2UBLTR.SoftNetWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;

namespace SoftNetNef2UBLTR
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Kullanılan UBLTR Kütüphane : https://github.com/hkutluay/UblTr

            var nefEarsiv = SoftNetOP.GetEarsiv();

            #region Vergiler

            var taxTotal = new UblTr.Common.TaxTotalType[]
            {
                new UblTr.Common.TaxTotalType()
                {
                    TaxAmount = new UblTr.Common.TaxAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.KDVTutari),
                        currencyID = nefEarsiv.DovizTipi,
                    },
                    TaxSubtotal = new UblTr.Common.TaxSubtotalType[nefEarsiv.Vergiler.Length]
                }
            };

            for (var i = 0; i < nefEarsiv.Vergiler.Length; i++)
            {
                taxTotal[0].TaxSubtotal[i] = new UblTr.Common.TaxSubtotalType()
                {
                    TaxableAmount = new UblTr.Common.TaxableAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.Vergiler[i].Matrah),
                        currencyID = nefEarsiv.DovizTipi,
                    },
                    TaxAmount = new UblTr.Common.TaxAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.Vergiler[i].Tutar),
                        currencyID = nefEarsiv.DovizTipi,
                    },
                    Percent = Convert.ToDecimal(nefEarsiv.Vergiler[i].Oran),
                    TaxCategory = new UblTr.Common.TaxCategoryType()
                    {
                        TaxScheme = new UblTr.Common.TaxSchemeType()
                        {
                            // Türe Vergi Adı Getirilmeli
                            // Item0015 : GERÇEK USULDE KATMA DEĞER VERGİSİ
                            Name = nefEarsiv.Vergiler[i].Tur.ToString(),
                            TaxTypeCode = nefEarsiv.Vergiler[i].Tur.ToString().Replace("Item", ""),
                        }
                    }
                };
            }

            #endregion

            #region Kalemler

            var invoiceLines = new UblTr.Common.InvoiceLineType[nefEarsiv.FaturaKalemleri.Length];

            for (var i = 0; i < nefEarsiv.FaturaKalemleri.Length; i++)
            {
                var kalem = nefEarsiv.FaturaKalemleri[i];
                invoiceLines[i] = new UblTr.Common.InvoiceLineType()
                {
                    ID = new UblTr.Common.IDType() { Value = (i + 1).ToString() },
                    InvoicedQuantity = new UblTr.Common.InvoicedQuantityType()
                    {
                        unitCode = kalem.Birim,
                        Value = Convert.ToDecimal(kalem.Miktar),
                    },
                    LineExtensionAmount = new UblTr.Common.LineExtensionAmountType()
                    {
                        Value = Convert.ToDecimal(kalem.Vergiler[0].Matrah),
                        currencyID = nefEarsiv.DovizTipi
                    },
                    OrderLineReference = new UblTr.Common.OrderLineReferenceType[]
                    {
                        new UblTr.Common.OrderLineReferenceType()
                        {
                            LineID = new UblTr.Common.LineIDType()
                            {
                                Value = (i + 1).ToString(),
                            }
                        }
                    },
                    TaxTotal = new UblTr.Common.TaxTotalType()
                    {
                        TaxAmount = new UblTr.Common.TaxAmountType()
                        {
                            Value = Convert.ToDecimal(kalem.Vergiler[0].Tutar),
                            currencyID = nefEarsiv.DovizTipi,
                        },
                        TaxSubtotal = new UblTr.Common.TaxSubtotalType[]
                        {
                            new UblTr.Common.TaxSubtotalType()
                            {
                                TaxableAmount = new UblTr.Common.TaxableAmountType()
                                {
                                    Value = Convert.ToDecimal(kalem.Vergiler[0].Matrah),
                                    currencyID = kalem.DovizTipi,
                                },
                                TaxAmount = new UblTr.Common.TaxAmountType()
                                {
                                    Value = Convert.ToDecimal(kalem.Vergiler[0].Tutar),
                                    currencyID = nefEarsiv.DovizTipi,
                                },
                                Percent = Convert.ToDecimal(kalem.Vergiler[0].Oran),
                                TaxCategory = new UblTr.Common.TaxCategoryType()
                                {
                                    TaxScheme = new UblTr.Common.TaxSchemeType()
                                    {
                                        // Türe Göre Vergi Adı Getirilmeli
                                        // Item0015 : GERÇEK USULDE KATMA DEĞER VERGİSİ
                                        Name = kalem.Vergiler[0].Tur.ToString(),
                                        TaxTypeCode = kalem.Vergiler[0].Tur.ToString().Replace("Item", ""),
                                    }
                                }
                            }
                        }
                    }
                };
            }

            #endregion

            var invoice = new UblTr.MainDoc.InvoiceType()
            {
                UUID = nefEarsiv.GUID,
                UBLVersionID = "2.1",
                CustomizationID = "TR1.2",
                ProfileID = nefEarsiv.Senaryo.ToString(),
                ID = nefEarsiv.No,
                CopyIndicator = false,
                AccountingSupplierParty = new UblTr.Common.SupplierPartyType()
                {
                    Party = new UblTr.Common.PartyType()
                    {
                        PartyIdentification = new UblTr.Common.PartyIdentificationType[]
                        {
                            new UblTr.Common.PartyIdentificationType()
                            {
                                ID = nefEarsiv.Tedarikci.VergiNoTCKimlikNo,
                            },
                        },
                        PartyName = new UblTr.Common.PartyNameType()
                        {
                            Name = nefEarsiv.Tedarikci.FirmaAdi
                        },
                        PostalAddress = new UblTr.Common.AddressType()
                        {
                            StreetName = nefEarsiv.Tedarikci.Sokak,
                            CitySubdivisionName = nefEarsiv.Tedarikci.IlceSemt,
                            CityName = nefEarsiv.Tedarikci.Il,
                            PostalZone = nefEarsiv.Tedarikci.PostaKodu,
                            Country = new UblTr.Common.CountryType()
                            {
                                Name = nefEarsiv.Tedarikci.Ulke
                            }
                        },
                        PartyTaxScheme = new UblTr.Common.PartyTaxSchemeType()
                        {
                            TaxScheme = new UblTr.Common.TaxSchemeType()
                            {
                                Name = nefEarsiv.Tedarikci.VergiDairesi
                            }
                        },
                        Person = new UblTr.Common.PersonType()
                        {
                            // UYARI : Geçici Çözüm
                            FirstName = nefEarsiv.Tedarikci.FirmaAdi.Split(' ').First(), // İsim
                            FamilyName = nefEarsiv.Tedarikci.FirmaAdi.Split(' ').Last(), // Soyisim
                        }
                    }
                },
                AccountingCustomerParty = new UblTr.Common.CustomerPartyType()
                {
                    Party = new UblTr.Common.PartyType()
                    {
                        PartyIdentification = new UblTr.Common.PartyIdentificationType[]
                        {
                            new UblTr.Common.PartyIdentificationType()
                            {
                                ID = nefEarsiv.Musteri.VergiNoTCKimlikNo,
                            },
                        },
                        PartyName = new UblTr.Common.PartyNameType()
                        {
                            Name = nefEarsiv.Musteri.FirmaAdi
                        },
                        PostalAddress = new UblTr.Common.AddressType()
                        {
                            StreetName = nefEarsiv.Musteri.Sokak,
                            CitySubdivisionName = nefEarsiv.Musteri.IlceSemt,
                            CityName = nefEarsiv.Musteri.Il,
                            PostalZone = nefEarsiv.Musteri.PostaKodu,
                            Country = new UblTr.Common.CountryType()
                            {
                                Name = nefEarsiv.Musteri.Ulke
                            }
                        },
                        Person = new UblTr.Common.PersonType()
                        {
                            // UYARI : Geçici Çözüm
                            FirstName = nefEarsiv.Musteri.FirmaAdi.Split(' ')[0], // İsim
                            FamilyName = nefEarsiv.Musteri
                            .FirmaAdi.Substring(nefEarsiv.Musteri.FirmaAdi.IndexOf(' ') + 1) // Soyisim
                        }
                    }
                },

                TaxTotal = taxTotal,

                LegalMonetaryTotal = new UblTr.Common.MonetaryTotalType()
                {
                    LineExtensionAmount = new UblTr.Common.LineExtensionAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.KDVMatrahi),
                        currencyID = nefEarsiv.DovizTipi
                    },
                    TaxExclusiveAmount = new UblTr.Common.TaxExclusiveAmountType
                    {
                        Value = Convert.ToDecimal(nefEarsiv.KDVMatrahi),
                        currencyID = nefEarsiv.DovizTipi
                    },
                    TaxInclusiveAmount = new UblTr.Common.TaxInclusiveAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.OdenecekToplamTutar),
                        currencyID = nefEarsiv.DovizTipi
                    },
                    PayableAmount = new UblTr.Common.PayableAmountType()
                    {
                        Value = Convert.ToDecimal(nefEarsiv.OdenecekToplamTutar),
                        currencyID = nefEarsiv.DovizTipi
                    }
                },

                InvoiceLine = invoiceLines,

            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UblTr.MainDoc.InvoiceType));

            using (TextWriter writer = new StreamWriter(@"./sample.xml"))
            {
                xmlSerializer.Serialize(writer, invoice, new UblTr.Serialization.UblTrNamespaces());
            }
        }
    }
}
