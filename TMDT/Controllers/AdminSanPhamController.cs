using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;

namespace TMDT.Controllers
{
    public class AdminSanPhamController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();

        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<SanPham> all = data.SanPhams.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(all.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form, HttpPostedFileBase HinhAnh)
        {
            string tenSanPham = form["TenSanPham"];
            decimal donGia = Convert.ToDecimal(form["DonGia"]);
            decimal phiVanChuyen = Convert.ToDecimal(form["PhiVanChuyen"]);
            int idNSX = Convert.ToInt32(form["IDNSX"]);
            string alias = form["SanPhamAlias"];
            string moTa = form["MoTaSanPham"];

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenSanPham))
            {
                ViewBag.MessageFail += "Tên sản phẩm không hợp lệ. ";
            }
            if (Convert.ToInt32(donGia) % 500 != 0)
            {
                ViewBag.MessageFail += "Đơn giá không hợp lệ. ";
            }
            if (Convert.ToInt32(phiVanChuyen) % 500 != 0)
            {
                ViewBag.MessageFail += "Phí vận chuyển không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }

            string _FileName = "";
            try
            {
                if (HinhAnh.ContentLength > 0)
                {
                    string _folderPath = Path.Combine(Server.MapPath("~/Images/NSX/"));
                    // Determine whether the directory exists.
                    if (!Directory.Exists(_folderPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(_folderPath);
                    }

                    _FileName = Path.GetFileName(HinhAnh.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/SanPham/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        HinhAnh.SaveAs(_path);
                    }
                }
            }
            catch
            {
            }

            SanPham SanPham = new SanPham();
            SanPham.TenSanPham = tenSanPham;
            SanPham.HinhAnh = _FileName;
            SanPham.DonGia = donGia;
            SanPham.PhiVanChuyen = phiVanChuyen;
            SanPham.SoLuongDaDat = 0;
            SanPham.IDNSX = idNSX;
            SanPham.SanPhamAlias = alias;
            SanPham.MoTaSanPham = moTa;

            if (ModelState.IsValid)
            {
                data.SanPhams.InsertOnSubmit(SanPham);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Thêm sản phẩm [" + tenSanPham + "] thành công";
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == id);
            return View(SanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            string tenSanPham = form["TenSanPham"];
            decimal donGia = Convert.ToDecimal(form["DonGia"]);
            decimal phiVanChuyen = Convert.ToDecimal(form["PhiVanChuyen"]);
            int idSanPham = Convert.ToInt32(form["IDSanPham"]);
            string alias = form["SanPhamAlias"];
            string moTa = form["MoTaSanPham"];

            SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == idSanPham);

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenSanPham))
            {
                ViewBag.MessageFail += "Tên sản phẩm không hợp lệ. ";
            }
            if (Convert.ToInt32(donGia) % 500 != 0)
            {
                ViewBag.MessageFail += "Đơn giá không hợp lệ. ";
            }
            if (Convert.ToInt32(phiVanChuyen) % 500 != 0)
            {
                ViewBag.MessageFail += "Phí vận chuyển không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }

            try
            {
                if (fileUpload.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileUpload.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/SanPham/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        fileUpload.SaveAs(_path);
                    }
                    SanPham.HinhAnh = _FileName;
                }
            }
            catch
            {
            }

            if (ModelState.IsValid)
            {
                UpdateModel(SanPham);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Đã thay đổi thông tin sản phẩm [" + tenSanPham + "] thành công";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == id);
            return View(SanPham);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form, int? page)
        {
            int id = Convert.ToInt32(form["id"]);
            SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == id);
            //Nếu nhóm tồn tại
            if (SanPham != null)
            {
                if (ModelState.IsValid)
                {
                    //Xóa nhóm
                    data.SanPhams.DeleteOnSubmit(SanPham);
                    try
                    {
                        data.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        int countChiTietDatHang = data.ChiTietDatHangs.Count(i => i.IDSanPham == id);
                        ViewBag.IsError = true;
                        if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            ViewBag.ErrorBody = string.Format("Không thể xóa sản phẩm [{0}] do đã có đơn đặt hàng đặt sản phẩm này.", SanPham.TenSanPham, countChiTietDatHang);
                        }
                        else
                        {
                            ViewBag.ErrorBody = ex.ToString();
                        }
                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        List<SanPham> all = data.SanPhams.ToList();
                        return View("Index", all.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminSanPham");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_NSX(int? idNSX)
        {
            List<NSX> all = data.NSXes.OrderBy(i => i.TenNSX).ToList();
            if (idNSX != null)
            {
                ViewBag.idNSX = idNSX;
            }
            return PartialView(all);
        }
        //end class
    }
}