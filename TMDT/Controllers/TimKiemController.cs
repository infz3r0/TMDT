using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;
using PagedList.Mvc;

namespace TMDT.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TimKiem
        private TMDTDBDataContext data = new TMDTDBDataContext();
        [HttpPost]
        public ActionResult KetQuaTimKiem(int? page, FormCollection f)
        {
            if (string.IsNullOrWhiteSpace(f["txtTimkiem"]))
            {
                return RedirectToAction("Index", "Home");
            }
            string tukhoa = f["txtTimkiem"].ToString();
            ViewBag.TK = tukhoa;
            List<SanPham> lsKQTK = data.SanPhams.Where(n => n.TenSanPham.Contains(tukhoa)).ToList();
            int pageSize = 6;
            int pageNum = (page ?? 1);
            if (lsKQTK.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy";
                return View(data.SanPhams.OrderBy(n => n.TenSanPham).ToPagedList(pageNum, pageSize));
            }
            ViewBag.ThongBao = "Đã tìm thấy" + " " + lsKQTK.Count + " " + "kết quả";
            return View(lsKQTK.OrderBy(n => n.TenSanPham).ToPagedList(pageNum, pageSize));
        }
        [HttpGet]
        public ActionResult KetQuaTimKiem()
        {
            return RedirectToAction("Index", "KhongTen");
        }
    }
}