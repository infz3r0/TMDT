using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class KhongTenController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();
        // GET: KhongTen
        private List<SanPham> LaSanPhamMoi(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var sanphammoi = LaSanPhamMoi(12);
            return View(sanphammoi);
        }
        public ActionResult NhomSanPham(int id)
        {
            var nhomsanpham = from n in data.Nhoms select n;
            return PartialView(nhomsanpham);
        }
        //chi tiet sp
        //public ActionResult Details(int id)
        //{
        //    var sanpham = from s in data.SanPhams
        //                  where s.IDSanPham == id
        //                  select s;
        //    return View(sanpham.Single());
        //}
        public ActionResult Details(string spAlias)
        {
            if (string.IsNullOrEmpty(spAlias))
            {
                return RedirectToAction("Index");
            }
            SanPham sanPham = data.SanPhams.FirstOrDefault(x=>x.SanPhamAlias.Equals(spAlias));
            return View(sanPham);
        }

        [ChildActionOnly]
        public ActionResult PartialNavbarItem()
        {
            List<Nhom> all = data.Nhoms.ToList();
            return PartialView(all);
        }
        [ChildActionOnly]
        public ActionResult PartialDropdownLoai(int? idNhom)
        {
            if(idNhom==null)
            {
                return RedirectToAction("Index", "KhongTen");
            }
            List<Loai> loai = data.Loais.Where(i => i.IDNhom == idNhom).ToList();
            return PartialView(loai);
        }
        [ChildActionOnly]
        public ActionResult PartialDropdownNSX(int? idLoai)
        {
            if (idLoai == null)
            {
                return RedirectToAction("Index", "KhongTen");
            }
            List<NSX> nsx = data.NSXes.Where(i => i.IDLoai == idLoai).ToList();
            return PartialView(nsx);
        }
        //public ActionResult NSXtheoLoai(int? idLoai, int? page)
        //{
        //    if (idLoai == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<NSX> list = data.NSXes.Where(i => i.IDLoai == idLoai).ToList();
        //    int pageSize = 6;
        //    int pageNum = page ?? 1;
        //    return View(list.ToPagedList(pageNum, pageSize));
        //}
        //Url Fr
        public ActionResult NSXtheoLoai(string loaiAlias, int? page)
        {
            if (string.IsNullOrEmpty(loaiAlias))
            {
                return RedirectToAction("Index");
            }
            List<NSX> list = data.NSXes.Where(i => i.Loai.LoaiAlias.Equals(loaiAlias)).ToList();
            int pageSize = 6;
            int pageNum = page ?? 1;
            return View(list.ToPagedList(pageNum, pageSize));
        }

        //public ActionResult sanphantheonsx(int? idNSX, int? page)
        //{
        //    if(idNSX == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<SanPham> list = data.SanPhams.Where(i => i.IDNSX == idNSX).ToList();
        //    int pageSize = 6;
        //    int pageNum = page ?? 1;
        //    return View(list.ToPagedList(pageNum, pageSize));
        //}

        public ActionResult SanPhamTheoNSX(string nsxAlias, int? page)
        {
            if (string.IsNullOrEmpty(nsxAlias))
            {
                return RedirectToAction("Index");
            }
            List<SanPham> list = data.SanPhams.Where(i => i.NSX.NSXAlias.Equals(nsxAlias)).ToList();
            int pageSize = 6;
            int pageNum = page ?? 1;
            return View(list.ToPagedList(pageNum, pageSize));
        }
    }
}