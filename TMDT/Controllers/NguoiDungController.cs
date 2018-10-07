using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class NguoiDungController : Controller
    {

        TMDTDBDataContext data = new TMDTDBDataContext();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]

        public ActionResult DangKy(FormCollection collection, TaiKhoan tk)
        {
            var tendn = collection["TenDangNhap"];
            var matkhau = collection["MatKhau"];
            var nhaplaimatkhau = collection["NhapLaiMatKhau"];
            var hoten = collection["HoTen"];
            bool gioitinh = Convert.ToBoolean(Convert.ToInt16(collection["GioiTinh"]));
            var dienthoai = collection["DienThoai"];
            var email = collection["Email"];

            if (nhaplaimatkhau != matkhau)
            {
                ViewData["Loi1"] = "Mật khẩu không trùng khớp, vui lòng kiểm tra lại!";
            }
            else
            {
                tk.Username = tendn;
                tk.Password = matkhau;
                tk.HoTen = hoten;
                tk.GioiTinh = gioitinh;
                tk.Sdt = dienthoai;
                tk.Email = email;
                data.TaiKhoans.InsertOnSubmit(tk);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            return this.DangKy();
        }

        [HttpGet]

        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]

        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TenDangNhap"];
            var matkhau = collection["MatKhau"];

            TaiKhoan tk = data.TaiKhoans.FirstOrDefault(n => n.Username == tendn && n.Password == matkhau);
            if (tk != null)
            {
                ViewBag.ThongBao = "Đăng nhập thành công";
                Session["Username"] = tk;
                return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.Thongbao = "Sai thông tin đăng nhập, vui lòng kiểm tra lại!";
            return View();
        }
        public ActionResult HienTenDN(int id)
        {
            TaiKhoan tk = (TaiKhoan)Session["Username"];
            return PartialView(tk);
        }

        public ActionResult TenDN()
        {
            return View();
        }

        public ActionResult TaiKhoan()
        {
            TaiKhoan tk = (TaiKhoan)Session["Username"];
            return PartialView(tk);
        }

        public ActionResult DangXuat()
        {
            //TaiKhoan tk = (TaiKhoan)Session["Username"];
            Session["Username"] = null;
            return RedirectToAction("DangNhap", "NguoiDung");
        }

        public ActionResult ThongTinTK()
        {

            if (Session["Username"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            TaiKhoan tk = (TaiKhoan)Session["Username"];
            var tt = from m in data.TaiKhoans where m.IDUser == tk.IDUser select m;
            return View(tt);
        }
        [HttpGet]

        public ActionResult DoiMatKhau(int id)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(i => i.IDUser == id);
            return View(tk);

        }

        // POST: AdminCTV/Edit/5
        [HttpPost]
        public ActionResult DoiMatKhau(TaiKhoan tk, FormCollection collection)
        {
            string matkhaucu = collection["MatKhauCu"];
            string matkhaumoi = collection["MatKhauMoi"];
            
            TaiKhoan ctv = data.TaiKhoans.SingleOrDefault(i => i.IDUser == tk.IDUser);
            if (matkhaucu == ctv.Password)
            {
                ctv.Password = matkhaumoi;

                UpdateModel(ctv);
                data.SubmitChanges();

                Session["Username"] = null;
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            else
            {
                ViewBag.Loi = "Mật khẩu không hợp lệ !";
            }

            return View();

        }

    }
}