namespace OA.Core.UI.Html
{
    class HtmlImageList
    {
        public static HtmlImageList Empty = new HtmlImageList();
        readonly List<HtmlImage> m_Images = new List<HtmlImage>();

        public HtmlImage this[int index]
        {
            get
            {
                if (m_Images.Count == 0)
                    return null;
                if (index >= m_Images.Count)
                    index = m_Images.Count - 1;
                if (index < 0)
                    index = 0;
                return m_Images[index];
            }
        }

        public int Count
        {
            get { return m_Images.Count; }
        }

        public void AddImage(Rectangle area, Texture2D image)
        {
            m_Images.Add(new HtmlImage(area, image));
        }

        public void AddImage(Rectangle area, Texture2D image, Texture2D overimage, Texture2D downimage)
        {
            AddImage(area, image);
            m_Images[m_Images.Count - 1].TextureOver = overimage;
            m_Images[m_Images.Count - 1].TextureDown = downimage;
        }

        public void Clear()
        {
            foreach (HtmlImage image in m_Images)
            {
                image.Dispose();
            }
            m_Images.Clear();
        }
    }

    
}
