using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HB2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HB2.ViewModels;


namespace HB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserContext db;

        public HomeController(UserContext user)
        {
            db = user;
        }
        public IActionResult Index()
        {
            IQueryable<Operation> operat = db.Operations.Include(c => c.Plan);
            operat = operat.Where(p => p.Plan.User.Email == User.Identity.Name);
            operat = operat.Where(p => p.Plan.Data <= DateTime.Today && p.Plan.DataPeriod >= DateTime.Today);
            foreach(Operation op in operat)
            {
                if (op.p != null)
                {
                    foreach (P p in op.p)
                    {
                        op.Procent = +p.Sum;
                    }
                }
                //if (op.p.Count != 0)
                //{
                //    //    int pr = op.Sum / 100;// 1%
                //    //    int s = 0;
                //    foreach (P p in op.p)
                //    {
                //       op.Procent= +p.Sum;
                //    }
                //    //op.Procent = s / pr;
                //}
                //else
                //{
                //    op.Procent = 0;
                //}

            }
            return View(operat);
        }
        [HttpGet]
        public IActionResult Make(int id)
        {
            Mod1 mod1 = new Mod1();
            mod1.Id = id;
            return View(mod1);
        }
        [HttpPost]
        public IActionResult Make(P p)
        {
            db.Ps.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult MakeOp(int id)
        {
            Mod1 mod1 = new Mod1();
            mod1.Id = id;
            return View(mod1);
        }
        [HttpPost]
        public IActionResult MakeOp(P p)
        {
            db.Ps.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Plan()
        {
            IQueryable<Plan> pl = db.Plans.Include(c => c.User);
            pl = pl.Where(p => p.User.Email == User.Identity.Name);
            return View(pl);
        }

        public IActionResult OperPlan(int id)
        {
            IQueryable<Operation> operat = db.Operations.Include(c => c.Plan);
            operat = operat.Where(p => p.PlanId == id);
            RedactPl pl = new RedactPl();
            pl.Opers = operat;
            pl._Id = id;
            return View(pl);
        }


        public IActionResult AddPlan()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPlan(Plan plan)
        {
            db.Plans.Add(plan);
            db.SaveChanges();
            return RedirectToAction("Plan");
        }
        [HttpGet]
        public IActionResult RedactOper(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);

            return View(op);
        }
        [HttpPost]
        public IActionResult RedactOper(Operation op)
        {
            db.Operations.Update(op);
            db.SaveChanges();
            return RedirectToAction("OperPlan", new { id = op.PlanId });
        }
        [HttpGet]
        public IActionResult AddOperation(int id)
        {
            Id id1 = new Id();
            id1._Id = id;
            return View(id1);
        }
        [HttpPost]
        public IActionResult AddOperation(Operation op)
        {

            db.Operations.Add(op);
            db.SaveChanges();
            return RedirectToAction("OperPlan", new { id = op.PlanId });
        }

        public IActionResult Filter(Operation operation)
        {
            IQueryable<Operation> oper = db.Operations.Include(c => c.Plan.User);
            oper = oper.Where(p => p.Plan.User.Email == User.Identity.Name);
            oper = oper.Where(a => a.NameAct == operation.NameAct);
            //if (operation.Name != null)
            //{
            //    oper = oper.Where(a => a.Name == operation.Name);

            //    if (operation.Data != null)
            //    {
            //        oper = oper.Where(a => a.Data == operation.Data);
            //        if (operation.View != null)
            //        {
            //            oper = oper.Where(a => a.View == operation.View);
            //            if (operation.Type != null)
            //            {
            //                oper = oper.Where(a => a.Type == operation.Type);
            //                if (operation.Sum != null)
            //                {
            //                    oper = oper.Where(a => a.Sum == operation.Sum);
            //                    if (operation.Coment != null)
            //                    {
            //                        oper = oper.Where(a => a.Coment == operation.Coment);
            //                        return View(oper);
            //                    }
            //                    return View(oper);
            //                }
            //                if (operation.Coment != null)
            //                {
            //                    oper = oper.Where(a => a.Coment == operation.Coment);
            //                    return View(oper);
            //                }
            //                return View(oper);
            //            }
            //            if (operation.Sum != null)
            //            {
            //                oper = oper.Where(a => a.Sum == operation.Sum);
            //                if (operation.Coment != null)
            //                {
            //                    oper = oper.Where(a => a.Coment == operation.Coment);
            //                    return View(oper);
            //                }
            //                return View(oper);
            //            }
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Type != null)
            //        {
            //            oper = oper.Where(a => a.Type == operation.Type);
            //            if (operation.Sum != null)
            //            {
            //                oper = oper.Where(a => a.Sum == operation.Sum);
            //                if (operation.Coment != null)
            //                {
            //                    oper = oper.Where(a => a.Coment == operation.Coment);
            //                    return View(oper);
            //                }
            //                return View(oper);
            //            }
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.View != null)
            //    {
            //        oper = oper.Where(a => a.View == operation.View);
            //        if (operation.Type != null)
            //        {
            //            oper = oper.Where(a => a.Type == operation.Type);
            //            if (operation.Sum != null)
            //            {
            //                oper = oper.Where(a => a.Sum == operation.Sum);
            //                if (operation.Coment != null)
            //                {
            //                    oper = oper.Where(a => a.Coment == operation.Coment);
            //                    return View(oper);
            //                }
            //                return View(oper);
            //            }
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Type != null)
            //    {
            //        oper = oper.Where(a => a.Type == operation.Type);
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Sum != null)
            //    {
            //        oper = oper.Where(a => a.Sum == operation.Sum);
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Coment != null)
            //    {
            //        oper = oper.Where(a => a.Coment == operation.Coment);
            //        return View(oper);
            //    }
            //    return View(oper);
            //}
            //if (operation.Data != null)
            //{
            //    oper = oper.Where(a => a.Data == operation.Data);
            //    if (operation.View != null)
            //    {
            //        oper = oper.Where(a => a.View == operation.View);
            //        if (operation.Type != null)
            //        {
            //            oper = oper.Where(a => a.Type == operation.Type);
            //            if (operation.Sum != null)
            //            {
            //                oper = oper.Where(a => a.Sum == operation.Sum);
            //                if (operation.Coment != null)
            //                {
            //                    oper = oper.Where(a => a.Coment == operation.Coment);
            //                    return View(oper);
            //                }
            //                return View(oper);
            //            }
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Type != null)
            //    {
            //        oper = oper.Where(a => a.Type == operation.Type);
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Sum != null)
            //    {
            //        oper = oper.Where(a => a.Sum == operation.Sum);
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Coment != null)
            //    {
            //        oper = oper.Where(a => a.Coment == operation.Coment);
            //        return View(oper);
            //    }
            //    return View(oper);
            //}
            //if (operation.View != null)
            //{
            //    oper = oper.Where(a => a.View == operation.View);
            //    if (operation.Type != null)
            //    {
            //        oper = oper.Where(a => a.Type == operation.Type);
            //        if (operation.Sum != null)
            //        {
            //            oper = oper.Where(a => a.Sum == operation.Sum);
            //            if (operation.Coment != null)
            //            {
            //                oper = oper.Where(a => a.Coment == operation.Coment);
            //                return View(oper);
            //            }
            //            return View(oper);
            //        }
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Sum != null)
            //    {
            //        oper = oper.Where(a => a.Sum == operation.Sum);
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Coment != null)
            //    {
            //        oper = oper.Where(a => a.Coment == operation.Coment);
            //        return View(oper);
            //    }
            //    return View(oper);
            //}
            //if (operation.Type != null)
            //{
            //    oper = oper.Where(a => a.Type == operation.Type);
            //    if (operation.Sum != null)
            //    {
            //        oper = oper.Where(a => a.Sum == operation.Sum);
            //        if (operation.Coment != null)
            //        {
            //            oper = oper.Where(a => a.Coment == operation.Coment);
            //            return View(oper);
            //        }
            //        return View(oper);
            //    }
            //    if (operation.Coment != null)
            //    {
            //        oper = oper.Where(a => a.Coment == operation.Coment);
            //        return View(oper);
            //    }
            //    return View(oper);
            //}
            //if (operation.Sum != null)
            //{
            //    oper = oper.Where(a => a.Sum == operation.Sum);
            //    if (operation.Coment != null)
            //    {
            //        oper = oper.Where(a => a.Coment == operation.Coment);
            //        return View(oper);
            //    }
            //    return View(oper);
            //}
            //if (operation.Coment != null)
            //{
            //    oper = oper.Where(a => a.Coment == operation.Coment);
            //    return View(oper);
            //}
            return View(oper);

        }



    }
}
