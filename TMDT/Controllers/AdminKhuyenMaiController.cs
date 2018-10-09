using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;

namespace TMDT.Controllers
{
    public class AdminKhuyenMaiController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();

        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }
            List<KhuyenMai> all = data.KhuyenMais.ToList();

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
        public ActionResult Create(FormCollection form)
        {
            int idSanPham = Convert.ToInt32(form["IDSanPham"]);
            double giamGia = Convert.ToDouble(form["GiamGia"]);

            ViewBag.MessageFail = string.Empty;
            if (idSanPham == 0)
            {
                ViewBag.MessageFail += "Không có sản phẩm để thêm. ";
            }
            if (giamGia < 0 || giamGia > 100)
            {
                ViewBag.MessageFail += "% Khuyến mại không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View();
            }

            KhuyenMai khuyenMai = new KhuyenMai();
            khuyenMai.IDSanPham = idSanPham;
            khuyenMai.GiamGia = giamGia;

            if (ModelState.IsValid)
            {
                data.KhuyenMais.InsertOnSubmit(khuyenMai);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY constraint"))
                    {
                        ViewBag.MessageFail = string.Format("Đã tồn tại khuyến mại cho sản phẩm {0}.", khuyenMai.SanPham.TenSanPham);
                    }
                    else
                    {
                        ViewBag.ErrorBody = ex.ToString();
                    }
                    return View();
                }
            }

            ViewBag.MessageSuccess = "Thêm khuyến mại: [" + khuyenMai.SanPham.TenSanPham + "] thành công";
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
            KhuyenMai khuyenMai = data.KhuyenMais.SingleOrDefault(i => i.IDSanPham == id);
            return View(khuyenMai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            int idSanPham = Convert.ToInt32(form["IDSanPham"]);
            double giamGia = Convert.ToDouble(form["GiamGia"]);

            KhuyenMai km = data.KhuyenMais.SingleOrDefault(i => i.IDSanPham == idSanPham);

            ViewBag.MessageFail = string.Empty;
            if (idSanPham == 0)
            {
                ViewBag.MessageFail += "Không có sản phẩm để thêm. ";
            }
            if (giamGia < 0 || giamGia > 100)
            {
                ViewBag.MessageFail += "% Khuyến mại không hợp lệ. ";
            }
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View(km);
            }

            if (ModelState.IsValid)
            {
                UpdateModel(km);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY constraint"))
                    {
                        ViewBag.MessageFail = string.Format("Đã tồn tại khuyến mại cho sản phẩm {0}.", km.SanPham.TenSanPham);
                    }
                    else
                    {
                        ViewBag.ErrorBody = ex.ToString();
                    }
                    return View();
                }
            }

            ViewBag.MessageSuccess = "Đã thay đổi thông tin khuyến mại cho [" + km.SanPham.TenSanPham + "] thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            int id = Convert.ToInt32(form["id"]);
            KhuyenMai khuyenMai = data.KhuyenMais.SingleOrDefault(i => i.IDSanPham == id);
            //Nếu nhóm tồn tại
            if (khuyenMai != null)
            {
                //Xóa nhóm
                data.KhuyenMais.DeleteOnSubmit(khuyenMai);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    //int countSanPham = data.SanPhams.Count(i => i.IDMonAn == id);
                    //ViewBag.IsError = true;
                    //if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    //{
                    //    ViewBag.ErrorBody = string.Format("Không thể xóa món ăn [{0}] do có {1} sản phẩm trong món ăn này.", monAn.TenMonAn, countSanPham);
                    //}
                    //else
                    //{
                    ViewBag.ErrorBody = ex.ToString();
                    //}
                    List<KhuyenMai> all = data.KhuyenMais.ToList();
                    return View("Index", all);
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminKhuyenMai");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_SanPham(int? idSanPham)
        {
            List<SanPham> all = data.SanPhams.OrderBy(i => i.TenSanPham).ToList();
            if (idSanPham != null)
            {
                ViewBag.idSanPham = idSanPham;
            }
            return PartialView(all);
        }

        [HttpPost]
        public ActionResult SanPhamById(int? val)
        {
            if (val != null)
            {
                //Values are hard coded for demo. you may replae with values 
                // coming from your db/service based on the passed in value ( val.Value)
                int id = val.Value;
                SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == id);
                string donGia = Convert.ToDecimal(SanPham.DonGia) + " USD";
                return Json(new { Success = "true", Data = new { DonGia = donGia } });
            }
            return Json(new { Success = "false" });
        }

        [HttpPost]
        public ActionResult GetGiaKhuyenMai(int? idSanPham, int? val)
        {
            if (val != null && idSanPham != null)
            {
                //Values are hard coded for demo. you may replae with values 
                // coming from your db/service based on the passed in value ( val.Value)
                double percent = Convert.ToDouble(val.Value) / 100;
                int id = idSanPham.Value;
                SanPham SanPham = data.SanPhams.SingleOrDefault(i => i.IDSanPham == id);
                if (SanPham != null)
                {
                    decimal giaGiam = Convert.ToDecimal(SanPham.DonGia) * Convert.ToDecimal(percent);
                    string giaKhuyenMai = (Convert.ToDecimal(SanPham.DonGia) - giaGiam) + " USD";
                    return Json(new { Success = "true", Data = new { GiaKhuyenMai = giaKhuyenMai } });
                }
                else
                {
                    return Json(new { Success = "false" });
                }

            }
            return Json(new { Success = "false" });
        }
        //end class
    }
}