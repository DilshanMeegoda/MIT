using System;
using Android.Content;
using Android.Util;
using Android.Views;

namespace WorkFlowManagement.CustomViews
{
    public class FlowLayout : ViewGroup
    {
        private int line_height;

        public class FlowLayoutParams : LayoutParams
        {
            public int horizontal_spacing;
            public int vertical_spacing;

            public FlowLayoutParams(int width, int height) : base(0, 0)
            {
                this.horizontal_spacing = horizontal_spacing;
                this.vertical_spacing = vertical_spacing;
            }
        }

        public FlowLayout(Context context) : base(context) { }

        public FlowLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = MeasureSpec.GetSize(widthMeasureSpec) - PaddingLeft - PaddingRight;
            int height = MeasureSpec.GetSize(heightMeasureSpec) - PaddingTop - PaddingBottom;
            int count = ChildCount;
            int line_height = 0;

            int xPosition = PaddingLeft;
            int yPosition = PaddingTop;

            int childHeightMeasureSpec;
            if (MeasureSpec.GetMode(heightMeasureSpec) == MeasureSpecMode.AtMost)
            {
                childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.AtMost);
            }
            else
            {
                childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            }


            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone)
                {
                    var lp = (FlowLayoutParams)child.LayoutParameters;
                    //var lp = child.LayoutParameters;
                    child.Measure(MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.AtMost), childHeightMeasureSpec);
                    int childw = child.MeasuredWidth;
                    line_height = Math.Max(line_height, child.MeasuredHeight + lp.vertical_spacing);

                    if (xPosition + childw > width)
                    {
                        xPosition = PaddingLeft;
                        yPosition += line_height;
                    }

                    xPosition += childw + lp.horizontal_spacing;
                }
            }
            this.line_height = line_height;

            if (MeasureSpec.GetMode(heightMeasureSpec) == MeasureSpecMode.Unspecified)
            {
                height = yPosition + line_height;

            }
            else if (MeasureSpec.GetMode(heightMeasureSpec) == MeasureSpecMode.AtMost)
            {
                if (yPosition + line_height < height)
                {
                    height = yPosition + line_height;
                }
            }
            SetMeasuredDimension(width, height);
        }

        protected override LayoutParams GenerateDefaultLayoutParams()
        {
            return new FlowLayoutParams(1, 1); // default of 1px spacing
        }

        protected override bool CheckLayoutParams(LayoutParams p)
        {
            if (new FlowLayoutParams(0, 0).Class.IsAssignableFrom(p.Class))
            {
                return true;
            }
            return false;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int count = ChildCount;
            int width = r - l;
            int xPosition = PaddingLeft;
            int yPosition = PaddingTop;

            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone)
                {
                    int childw = child.MeasuredWidth;
                    int childh = child.MeasuredHeight;
                    var lp = (FlowLayoutParams)child.LayoutParameters;
                    if (xPosition + childw > width)
                    {
                        xPosition = PaddingLeft;
                        yPosition += line_height;
                    }
                    child.Layout(xPosition, yPosition, xPosition + childw, yPosition + childh);
                    xPosition += childw + lp.horizontal_spacing;
                }
            }
        }
    }
}