using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;

namespace WorkFlowManagement.CustomClasses
{
    public class MiniGallerAdapter : BaseAdapter<CustomGallery>
    {

        public class ViewHolder : Java.Lang.Object
        {
            public ImageView ImgQueue
            {
                get;
                set;
            }

            public ImageView ImgQueueMultiSelected
            {
                get;
                set;
            }
        }

        private class SimpleImageLoadingListenerImpl : SimpleImageLoadingListener
        {
            public SimpleImageLoadingListenerImpl(GalleryAdapter.ViewHolder holder)
            {
                this.holder = holder;
            }

            GalleryAdapter.ViewHolder holder;

            public override void OnLoadingStarted(String imageUri, View view)
            {
                holder.
                ImgQueue.
                SetImageResource(Resource.Drawable.no_media);

                base.OnLoadingStarted(imageUri, view);
            }
        }

        private Context mContext;
        private LayoutInflater inflater;
        private List<CustomGallery> data;
        ImageLoader imageLoader;

        public MiniGallerAdapter(Context c, ImageLoader imageLoader)
        {

            this.data = new List<CustomGallery>();
            this.inflater = (LayoutInflater)c.GetSystemService(Context.LayoutInflaterService);
            this.mContext = c;

            this.imageLoader = imageLoader;
            // clearCache();
        }

        public override int Count
        {
            get
            {
                return data.Count;
            }
        }

        public override CustomGallery this[int index]
        {
            get
            {
                return data[index];
            }
        }

        public List<CustomGallery> getList()
        {
            return data;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public bool AnySelected
        {
            get
            {
                return data.Any(x => x.IsSelected);
            }
        }

        public IEnumerable<CustomGallery> Selected
        {
            get
            {
                return
                    data.
                    Where(x => x.IsSelected).
                    ToList();
            }
        }

        public void AddAll(IEnumerable<CustomGallery> files)
        {

            try
            {
                this.data.Clear();
                this.data.AddRange(files);

            }
            catch (Exception e)
            {
                throw;
            }

            NotifyDataSetChanged();
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            GalleryAdapter.ViewHolder holder;

            if (convertView == null)
            {

                convertView = inflater.Inflate(Resource.Layout.mini_galary_item, null);

                holder = new GalleryAdapter.ViewHolder();

                holder.ImgQueue = (ImageView)convertView.FindViewById(Resource.Id.imgQueue2);

                convertView.Tag = holder;

            }
            else
            {
                holder = (GalleryAdapter.ViewHolder)convertView.Tag;
            }

            holder.ImgQueue.Tag = position;

            try
            {

                imageLoader.DisplayImage("file://" + data[position].SdCardPath, holder.ImgQueue,
                    new MiniGallerAdapter.SimpleImageLoadingListenerImpl(holder));


            }
            catch (Exception e)
            {
                throw;
            }

            return convertView;
        }


        public void ClearCache()
        {
            imageLoader.ClearDiskCache();
            imageLoader.ClearMemoryCache();
        }

        public void Clear()
        {
            data.Clear();
            NotifyDataSetChanged();
        }
    }
}
