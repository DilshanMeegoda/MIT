using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WorkFlowManagement.Common
{
    public class RoundedImageView : ImageView
    {
        protected RoundedImageView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public RoundedImageView(Context context)
            : base(context)
        {
        }

        public RoundedImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public RoundedImageView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        public RoundedImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {

            float radius = 150.0f; // angle of round corners
            Path clipPath = new Path();
            RectF rect = new RectF(0, 0, this.Width, this.Height);
            clipPath.AddRoundRect(rect, radius, radius, Path.Direction.Cw);
            canvas.ClipPath(clipPath);
            base.OnDraw(canvas);
        }

    }
}