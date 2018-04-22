using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI.Html
{
    class HtmlImageList
    {
        public static HtmlImageList Empty = new HtmlImageList();

        readonly List<HtmlImage> _images = new List<HtmlImage>();

        public HtmlImage this[int index]
        {
            get
            {
                if (_images.Count == 0)
                    return null;
                if (index >= _images.Count)
                    index = _images.Count - 1;
                if (index < 0)
                    index = 0;
                return _images[index];
            }
        }

        public int Count
        {
            get { return _images.Count; }
        }

        public void AddImage(RectInt area, Texture2D image)
        {
            _images.Add(new HtmlImage(area, image));
        }

        public void AddImage(RectInt area, Texture2D image, Texture2D overimage, Texture2D downimage)
        {
            AddImage(area, image);
            _images[_images.Count - 1].TextureOver = overimage;
            _images[_images.Count - 1].TextureDown = downimage;
        }

        public void Clear()
        {
            foreach (var image in _images)
                image.Dispose();
            _images.Clear();
        }
    }

    
}
