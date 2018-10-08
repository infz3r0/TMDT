using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;

namespace TMDT.Controllers
{
    public class AdminLoaiController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();

        // GET: AdminLoai
        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<Loai> all = data.Loais.ToList();

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
            string tenLoai = form["TenLoai"];
            int idNhom = Convert.ToInt32(form["IDNhom"]);
            string alias = form["LoaiAlias"];

            if (string.IsNullOrWhiteSpace(tenLoai))
            {
                ViewBag.MessageFail = "Tên loại không hợp lệ";
                return View();
            }

            Loai loai = new Loai();
            loai.TenLoai = tenLoai;
            loai.IDNhom = idNhom;
            loai.SoLuong = 0;
            loai.LoaiAlias = alias;

            if (ModelState.IsValid)
            {
                data.Loais.InsertOnSubmit(loai);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Thêm loại: [" + tenLoai + "] thành công";
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
            Loai loai = data.Loais.SingleOrDefault(i => i.IDLoai == id);
            return View(loai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            string tenLoai = form["TenLoai"];
            int id = Convert.ToInt32(form["IDLoai"]);
            string alias = form["LoaiAlias"];
            Loai loai = data.Loais.SingleOrDefault(i => i.IDLoai == id);

            if (string.IsNullOrWhiteSpace(tenLoai))
            {
                ViewBag.MessageFail = "Tên nhóm không hợp lệ";
                return View(loai);
            }

            if (ModelState.IsValid)
            {
                UpdateModel(loai);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Đã thay đổi thông tin loại [" + tenLoai + "] thành công";
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
            Loai loai = data.Loais.SingleOrDefault(i => i.IDLoai == id);
            return View(loai);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form, int? page)
        {
            int id = Convert.ToInt32(form["id"]);
            Loai loai = data.Loais.SingleOrDefault(i => i.IDLoai == id);
            //Nếu nhóm tồn tại
            if (loai != null)
            {
                if (ModelState.IsValid)
                {
                    //Xóa nhóm
                    data.Loais.DeleteOnSubmit(loai);
                    try
                    {
                        data.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        int countNSX = data.NSXes.Count(i => i.IDLoai == id);
                        ViewBag.IsError = true;
                        if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            ViewBag.ErrorBody = string.Format("Không thể xóa loại [{0}] do có {1} NSX trong loại này.", loai.TenLoai, countNSX);
                        }
                        else
                        {
                            ViewBag.ErrorBody = ex.ToString();
                        }
                        int pageSize = 10;
                        int pageNumber = (page ?? 1);
                        List<Loai> all = data.Loais.ToList();
                        return View("Index", all.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminLoai");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_Nhom(int? idNhom)
        {
            List<Nhom> all = data.Nhoms.ToList();
            if (idNhom != null)
            {
                ViewBag.idNhom = idNhom;
            }
            return PartialView(all);
        }
        //end class
    }
}