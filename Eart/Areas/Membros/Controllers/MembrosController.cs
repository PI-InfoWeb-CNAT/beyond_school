﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Eart.Areas.Membros.Models;
using Eart.Persistencia.DAL;

namespace Eart.Areas.Membros.Controllers
{
    public class MembrosController : Controller
    {
        MembroDAL membroDAL = new MembroDAL();
        SeguirDAL seguirDAL = new SeguirDAL();

        private ActionResult ObterVisaoMembroPorId(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Membro membro = membroDAL.ObterMembroPorId((long) id);
            if (membro == null)
            {
                return HttpNotFound();
            }
            return View(membro);
        }

        private byte[] SetFotoPerfil(HttpPostedFileBase fotoPerfil)
        {
            var bytesFotoPerfil = new byte[fotoPerfil.ContentLength];
            fotoPerfil.InputStream.Read(bytesFotoPerfil, 0, fotoPerfil.ContentLength);
            return bytesFotoPerfil;
        }

        public FileContentResult GetFotoPerfil(long id)
        {
            Membro membro = membroDAL.ObterMembroPorId(id);
            if (membro != null)
            {
                return File(membro.FotoPerfil, membro.FotoPerfilType);
            }
            return null;
        }

        private byte[] SetFotoCapa(HttpPostedFileBase fotoCapa)
        {
            var bytesFotoCapa = new byte[fotoCapa.ContentLength];
            fotoCapa.InputStream.Read(bytesFotoCapa, 0, fotoCapa.ContentLength);
            return bytesFotoCapa;
        }

        public FileContentResult GetFotoCapa(long id)
        {
            Membro membro = membroDAL.ObterMembroPorId(id);
            if (membro != null)
            {
                return File(membro.FotoCapa, membro.FotoCapaType);
            }
            return null;
        }


        private ActionResult GravarMembro(Membro membro, HttpPostedFileBase fotoPerfil = null, HttpPostedFileBase fotoCapa = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (fotoPerfil != null)
                    {
                        membro.FotoPerfilType = fotoPerfil.ContentType;
                        membro.FotoPerfil = SetFotoPerfil(fotoPerfil);
                    }
                    if (fotoCapa != null)
                    {
                        membro.FotoCapaType = fotoCapa.ContentType;
                        membro.FotoCapa = SetFotoCapa(fotoCapa);
                    }
                    membroDAL.GravarMembro(membro);
                }
                return View(membro);
            }
            catch
            {
                return View(membro);
            }
        }

        public ActionResult Index()
        {
            Membro membroLogin = HttpContext.Session["membroLogin"] as Membro;
            if (membroLogin != null)
            {
                ViewBag.MembroLogado = membroLogin.MembroId;
            }
            return View(membroDAL.ObterMembrosClassificadosPorNome());
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Membro membro, HttpPostedFileBase fotoPerfil = null, HttpPostedFileBase fotoCapa = null)
        {
            if(ModelState.IsValid)
            {
                membro.Ativo = true;
                GravarMembro(membro, fotoPerfil, fotoCapa);
                HttpContext.Session["membroLogin"] = membro;
                return RedirectToAction("Details", "Membros", new { Area = "Membros", id = membro.MembroId });
            }
            else
            {
                return GravarMembro(membro, fotoPerfil, fotoCapa);
            }
        }

        // Get: Edit
        public ActionResult Edit(long? id)
        {
            return ObterVisaoMembroPorId(id);
        }

        //POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Membro membro, HttpPostedFileBase fotoPerfil = null, HttpPostedFileBase fotoCapa = null)
        {
            Membro membroLogin = HttpContext.Session["membroLogin"] as Membro;
            if (membroLogin != null)
            {
                ViewBag.MembroLogado = membroLogin.MembroId;
            }
            if (ModelState.IsValid)
            {
                GravarMembro(membro, fotoPerfil, fotoCapa);
                return RedirectToAction("Details", "Membros", new { Area = "Membros", id = membro.MembroId });
            }
            else
            {
                return GravarMembro(membro, fotoPerfil, fotoCapa);
            }
        }

        public ActionResult Details(long? id)
        {
            Membro membroLogin = HttpContext.Session["membroLogin"] as Membro;
            ViewBag.MembroLogado = membroLogin.MembroId;
            ViewBag.MembroVisualizado = id;
            membroLogin.Seguindo = seguirDAL.ObterMembroSeguido((long)id, (long)membroLogin.MembroId);
            GravarMembro(membroLogin);
<<<<<<< HEAD
            if(membroLogin.MembroId != id) {
               Membro membroVisto = membroDAL.ObterMembroPorId((long)id);
               membroVisto.Seguindo = membroLogin.Seguindo;
               GravarMembro(membroVisto);
            }
=======
>>>>>>> main
            return ObterVisaoMembroPorId(id);
        }

        public ActionResult Delete(long? id)
        {
            return ObterVisaoMembroPorId(id);
        }

        // POST: Produtos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id, FormCollection collection)
        {
            try
            {
                Membro membroLogin = HttpContext.Session["membroLogin"] as Membro;
                if (membroLogin != null)
                {
                   ViewBag.MembroLogado = membroLogin.MembroId;
                }
                Membro membro = membroDAL.ObterMembroPorId(id);
                membro.Ativo = false;
                GravarMembro(membro);
                TempData["Message"] = "Membro " + membro.Nome.ToUpper() + " foi removido";
                return RedirectToAction("Create", "Membros", new { Area = "Membros" });
            }
            catch
            {
                return View();
            }
        }

    }
}