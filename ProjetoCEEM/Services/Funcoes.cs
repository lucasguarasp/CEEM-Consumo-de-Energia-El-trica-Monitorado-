using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Services
{
    public class Funcoes
    {
        public static string Upload(HttpPostedFileBase flpUpload)
        {

            string extensao = Path.GetExtension(flpUpload.FileName);
            if (extensao != ".jpg" && extensao != ".gif" && extensao != ".png")
                return "Arquivo inválido, por favor selecione somente arquivos do tipo JPG, GIF ou PNG!";
            else
            {
                string caminho = HttpContext.Current.Request.PhysicalApplicationPath + @"Content\ImagemPerfil\" + Path.GetFileName(flpUpload.FileName);
                flpUpload.SaveAs(caminho);
                //Criando imagens miniaturas
                try
                {   //para redimensionar a imagem utilizaremos o nome da imagem criando um novo objeto do tipo imagem
                    caminho = Path.GetFileName(flpUpload.FileName);
                    System.Drawing.Image imgOriginal = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/Content/ImagemPerfil/" + Path.GetFileName(flpUpload.FileName)), true);
                    System.Drawing.Image.GetThumbnailImageAbort miniatura = new System.Drawing.Image.GetThumbnailImageAbort(erro);
                    System.Drawing.Image imgRedimensionada;
                    int width;
                    int height;
                    ///Verifica se o width em px da imagem é maior que 200
                    if (imgOriginal.Size.Width > 200)
                    {
                        //Aqui é feita a renderização proporcional da altura da imagem
                        width = 600;
                        height = (int)(width * imgOriginal.Height) / imgOriginal.Width;
                    }
                    else
                    {
                        width = imgOriginal.Size.Width;
                        height = imgOriginal.Size.Height;
                    }
                    imgRedimensionada = imgOriginal.GetThumbnailImage(width, height, miniatura, IntPtr.Zero);
                    //Novo nome para imagem gerada
                    string novo_nome = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                    //Salve a miniatura em um formato.
                    imgRedimensionada.Save(HttpContext.Current.Request.PhysicalApplicationPath + @"Content\ImagemPerfil\" + novo_nome, System.Drawing.Imaging.ImageFormat.Png);
                    imgRedimensionada.Dispose();
                    imgOriginal.Dispose();
                    //Visualize a original e a redimensionada
                    File.Delete(HttpContext.Current.Server.MapPath("~/Content/ImagemPerfil/" + Path.GetFileName(flpUpload.FileName)));
                    return "sucesso|" + novo_nome;
                }
                catch (Exception ex)
                {
                    return "Erro: " + ex.ToString();
                }
            }
        }

        public static bool erro()
        {
            return false;
        }
    }
}