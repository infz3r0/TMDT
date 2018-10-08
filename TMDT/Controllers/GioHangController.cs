using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

using PayPal.Api;

namespace TMDT.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        TMDTDBDataContext data = new TMDTDBDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int iIDSanPham, string strURL)
        {
            List<GioHang> lstGiohang = LayGioHang();
            GioHang sanpham = lstGiohang.Find(n => n.iIDSanPham == iIDSanPham);
            if (sanpham == null)
            {
                sanpham = new GioHang(iIDSanPham);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        public double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            //----------------
            List<GioHang> lstGiohang = LayGioHang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "KhongTen");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaSP)
        {
            List<GioHang> lstGiohang = LayGioHang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iIDSanPham == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iIDSanPham == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "KhongTen");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<GioHang> lstGiohang = LayGioHang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iIDSanPham == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<GioHang> lstGiohang = LayGioHang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "KhongTen");
        }
        [HttpGet]

        public ActionResult DatHang()
        {
            if (Session["Username"] == null || Session["Username"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Nguoidung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "KhongTen");
            }
            List<GioHang> lstGiohang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.IDDatHang = data.f_GetIdentDatHang();
            return View(lstGiohang);

        }

        public ActionResult DatHang(FormCollection collection)
        {
            DatHang ddh = new DatHang();
            TaiKhoan tk = (TaiKhoan)Session["Username"];
            List<GioHang> gh = LayGioHang();
            ddh.IDUser = tk.IDUser;
            ddh.ThoiGianDatHang = DateTime.Now;
            ddh.DaGiaoHang = false;
            var diachigiaohang = collection["DiaChiGiaoHang"];
            ddh.DiaChiGiaoHang = diachigiaohang;
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhTien);
            }
            ddh.ThanhTien = decimal.Parse(iTongTien.ToString());
            data.DatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                ChiTietDatHang ctdn = new ChiTietDatHang();
                ctdn.ID = ddh.ID;

                ctdn.IDSanPham = item.iIDSanPham;
                ctdn.SoLuong = item.iSoLuong;
                data.ChiTietDatHangs.InsertOnSubmit(ctdn);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("Xacnhandonhang", "GioHang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }

        #region Paypal
        
        public ActionResult Paypal()
        {
            List<GioHang> gioHangs = LayGioHang();
            double shiptotal = 0;
            double sbtotal = 0;

            try
            {
                var apiContext = Configuration.GetAPIContext();
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    List<Item> items = new List<Item>();
                    foreach (GioHang o in gioHangs)
                    {
                        shiptotal += o.dPhiVanChuyen;
                        sbtotal += o.iSoLuong * o.dDonGia;
                        Item item = new Item()
                        {
                            name = o.sTenSanPham,
                            currency = "USD",
                            price = o.dDonGia.ToString() + ".00",
                            quantity = o.iSoLuong.ToString(),
                            sku = "sku"
                        };
                        items.Add(item);
                    }
                    var itemList = new ItemList();
                    itemList.items = items;


                    var payer = new Payer() { payment_method = "paypal" };


                    var baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/GioHang/Paypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var redirectUrl = baseURI + "guid=" + guid;
                    var redirUrls = new RedirectUrls()
                    {
                        cancel_url = redirectUrl + "&cancel=true",
                        return_url = redirectUrl
                    };


                    var details = new Details()
                    {
                        tax = "0.00",
                        shipping = shiptotal.ToString() + ".00",
                        subtotal = sbtotal.ToString() + ".00"
                    };


                    var amount = new Amount()
                    {
                        currency = "USD",
                        total = (shiptotal + sbtotal).ToString() + ".00", // Total must be equal to sum of shipping, tax and subtotal.
                        details = details
                    };


                    var transactionList = new List<Transaction>();


                    transactionList.Add(new Transaction()
                    {
                        description = "Transaction description.",
                        invoice_number = data.f_GetIdentDatHang().ToString(),
                        amount = amount,
                        item_list = itemList
                    });


                    var payment = new Payment()
                    {
                        intent = "sale",
                        payer = payer,
                        transactions = transactionList,
                        redirect_urls = redirUrls
                    };


                    var createdPayment = payment.Create(apiContext);


                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectURL = null;
                    while (links.MoveNext())
                    {
                        var link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //this.flow.RecordRedirectUrl("Redirect to PayPal to approve the payment...", link.href);
                            paypalRedirectURL = link.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    //Session.Add("flow-" + guid, this.flow);
                    return Redirect(paypalRedirectURL);
                }
                else
                {
                    var guid = Request.Params["guid"];


                    var paymentId = Session[guid] as string;
                    var paymentExecution = new PaymentExecution() { payer_id = payerId };
                    var payment = new Payment() { id = paymentId };


                    var executedPayment = payment.Execute(apiContext, paymentExecution);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("DatHang");
                    }

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("###Ex: " + ex.ToString());
                return RedirectToAction("DatHang");
            }

            return RedirectToAction("AfterSuccessPaypal");



        }
        #endregion

        public ActionResult AfterSuccessPaypal()
        {
            DatHang ddh = new DatHang();
            TaiKhoan tk = (TaiKhoan)Session["Username"];
            List<GioHang> gh = LayGioHang();
            ddh.IDUser = tk.IDUser;
            ddh.ThoiGianDatHang = DateTime.Now;
            ddh.DaGiaoHang = false;
            var diachigiaohang = "";
            ddh.DiaChiGiaoHang = diachigiaohang;
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhTien);
            }
            ddh.ThanhTien = decimal.Parse(iTongTien.ToString());
            data.DatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                ChiTietDatHang ctdn = new ChiTietDatHang();
                ctdn.ID = ddh.ID;

                ctdn.IDSanPham = item.iIDSanPham;
                ctdn.SoLuong = item.iSoLuong;
                data.ChiTietDatHangs.InsertOnSubmit(ctdn);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("Xacnhandonhang", "GioHang");
        }



        //end class
    }

}