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
    public class AdminNSXController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();

        // GET: AdminNSX
        public ActionResult Index(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<NSX> all = data.NSXes.ToList();

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
            string tenNSX = form["TenNSX"];
            string gioiThieu = form["GioiThieu"];
            int idLoai = Convert.ToInt32(form["IDLoai"]);
            string alias = form["NSXAlias"];

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenNSX))
            {
                ViewBag.MessageFail += "Tên NSX không hợp lệ. ";
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
                    string _path = Path.Combine(Server.MapPath("~/Images/NSX/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        HinhAnh.SaveAs(_path);
                    }
                }
            }
            catch
            {
            }

            NSX NSX = new NSX();
            NSX.TenNSX = tenNSX;
            NSX.GioiThieu = gioiThieu;
            NSX.HinhAnh = _FileName;
            NSX.IDLoai = idLoai;
            NSX.NSXAlias = alias;

            if (ModelState.IsValid)
            {
                data.NSXes.InsertOnSubmit(NSX);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Thêm NSX: [" + tenNSX + "] thành công";
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
            NSX NSX = data.NSXes.SingleOrDefault(i => i.IDNSX == id);
            return View(NSX);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            string tenNSX = form["TenNSX"];
            int id = Convert.ToInt32(form["IDNSX"]);
            string alias = form["NSXAlias"];

            NSX NSX = data.NSXes.SingleOrDefault(i => i.IDNSX == id);

            ViewBag.MessageFail = string.Empty;
            if (string.IsNullOrWhiteSpace(tenNSX))
            {
                ViewBag.MessageFail += "Tên NSX không hợp lệ. ";
            }
            
            if (!string.IsNullOrEmpty(ViewBag.MessageFail))
            {
                return View(NSX);
            }

            try
            {
                if (fileUpload.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileUpload.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Images/NSX/"), _FileName);
                    if (!System.IO.File.Exists(_path))
                    {
                        fileUpload.SaveAs(_path);
                    }
                    NSX.HinhAnh = _FileName;
                }
            }
            catch
            {
            }

            if (ModelState.IsValid)
            {
                UpdateModel(NSX);
                data.SubmitChanges();
            }
            ViewBag.MessageSuccess = "Đã thay đổi thông tin NSX [" + tenNSX + "] thành công";
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
            NSX NSX = data.NSXes.SingleOrDefault(i => i.IDNSX == id);
            return View(NSX);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form, int? page)
        {
            int id = Convert.ToInt32(form["id"]);
            NSX NSX = data.NSXes.SingleOrDefault(i => i.IDNSX == id);
            //Nếu nhóm tồn tại
            if (NSX != null)
            {
                //Xóa nhóm
                data.NSXes.DeleteOnSubmit(NSX);
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception ex)
                {
                    int countSanPham = data.SanPhams.Count(i => i.IDNSX == id);
                    ViewBag.IsError = true;
                    if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    {
                        ViewBag.ErrorBody = string.Format("Không thể xóa NSX [{0}] do có {1} sản phẩm trong NSX này.", NSX.TenNSX, countSanPham);
                    }
                    else
                    {
                        ViewBag.ErrorBody = ex.ToString();
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    List<NSX> all = data.NSXes.ToList();
                    return View("Index", all.ToPagedList(pageNumber, pageSize));
                }
            }
            //Xóa thành công hoặc nhóm k tồn tại thì trở về Index
            return RedirectToAction("Index", "AdminNSX");
        }

        [ChildActionOnly]
        public ActionResult PV_Dropdown_Loai(int? idLoai)
        {
            List<Loai> all = data.Loais.OrderBy(i => i.TenLoai).ToList();
            if (idLoai != null)
            {
                ViewBag.idLoai = idLoai;
            }
            return PartialView(all);
        }
        //end class
    }
}