using ImagesApi.Models;
using ImagesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using System;

namespace ImagesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ImageService _imageService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly String ServerName = "http://image.xingguosun.com/";
        public ImagesController(ImageService imageService, IHostingEnvironment hostingEnvironment)
        {
            _imageService = imageService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ActionResult<List<Image>> Get() =>
            _imageService.Get();

        [HttpGet("{id:length(24)}", Name = "GetImage")]
        public ActionResult<Image> Get(string id)
        {
            var image = _imageService.Get(id);

            if (image == null)
            {
                return NotFound();
            }

            return image;
        }

        [HttpPost]
        public ActionResult<Image> Create(Image image)
        {
            _imageService.Create(image);

            return CreatedAtRoute("GetImage", new { id = image.Id.ToString() }, image);
        }
        [HttpPost("UploadImage")]
        /// <summary>
        /// 图片上传并存入数据库
        /// </summary>
        /// <returns></returns>
        public JsonResult InsertPicture()
        {
            var uploadfile = Request.Form.Files[0];
            var now = DateTime.Now;
            var webRootPath = _hostingEnvironment.ContentRootPath;
            var filePath = string.Format("/Uploads/Images/{0}/{1}/",  now.ToString("yyyyMM"), now.ToString("yyyyMMdd"));

            if (!Directory.Exists(webRootPath + filePath))
            {
                Directory.CreateDirectory(webRootPath + filePath);
            }

            if (uploadfile != null)
            {
                //文件后缀
                var fileExtension = Path.GetExtension(uploadfile.FileName);

                //判断后缀是否是图片
                const string fileFilt = ".gif|.jpg|.php|.jsp|.jpeg|.png";
                if (fileExtension == null)
                {
                    return new JsonResult(new JsonResultModel { isSucceed = false, resultMsg = "上传的文件没有后缀" });
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    return new JsonResult(new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不是图片" });
                }

                //判断文件大小
                long length = uploadfile.Length;
                if (length > 1024*1024*2) //2M
                {
                    return new JsonResult(new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不能大于2M" });
                }

                var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                var saveName = strDateTime + strRan + fileExtension;

                // //插入图片数据
                var image = new Image
                {
                    // MimeType = uploadfile.ContentType,
                    // AltAttribute = "",
                    Url = filePath + saveName,
                    // CreatedDateTime = DateTime.Now
                };
                using (FileStream fs = System.IO.File.Create(webRootPath + filePath + saveName))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                // _pictureService.Insert(picture);
                return new JsonResult(new  {isSuccess = true, returnMsg = "上传成功", imgId = image.Id, imgUrl = image.Url});
            }
            return new JsonResult(new JsonResultModel { isSucceed = false, resultMsg = "上传失败" });
        }
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Image imageIn)
        {
            var image = _imageService.Get(id);

            if (image == null)
            {
                return NotFound();
            }

            _imageService.Update(id, imageIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var image = _imageService.Get(id);

            if (image == null)
            {
                return NotFound();
            }

            _imageService.Remove(image.Id);

            return NoContent();
        }
    }
}