using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;


namespace ConsoleAppPrint
{
    // I want to markout 8 drill holes.

    // this is a NON UI drill drawing print - Poor man’s CAD!
    // Why? I have a trusty laser printer - great for printing out arcane invoices for accountants that like paper copies.
    // But it's dimensionally not so good. Aspect ratio is out - so I cannot use a CAD package to scale by 1.04 by 1.05

    // This is a "hack it and bash it" so I've got something to use!


    class Program
    {
        static void Main(string[] args)
        {
            DrawShape d = new DrawShape();
            d.DrawRec();
        }

        class PlotLine
        {
            public PointF start { get; set; }
            public PointF end { get; set; }
        }

        class FootPrint
        {
            public List<PointF> DrillHoles = new List<PointF>();

            public SizeF offset { get; set; } // from pin 1
            public SizeF footprintSize { get; set; } // from pin 1
            public PointF pin1Position { get; set; }

            public float width { get; set; }// line width
        }


        class DrawShape
        {
            float xfactor = (50.8f / 48.5f) * (170f / 171f);
            float yfactor = (23.7f / 22.7f) * (170f / 169f) * (170f / 169.5f);

            public void DrawRec()
            {
                PrintDocument doc = new PrintDocument();
                doc.PrintPage += doc_PrintPage;
                doc.Print();
            }

            void doc_PrintPage(object sender, PrintPageEventArgs e)
            {
                Graphics g = e.Graphics;
                PageSettings PageSet = new PageSettings();
                
                g.PageUnit = GraphicsUnit.Millimeter;

                {
                    // box
                    FootPrint pf = new FootPrint();
                    pf.offset = new SizeF(0, 0);
                    pf.footprintSize = new SizeF(115, 90);
                    pf.pin1Position = new PointF(20.0F, 20.0F);
                    pf.width = 0.5f;

                    DrawFootPrint(g, pf);
                }

                {
                    // ESP32
                    FootPrint pf = new FootPrint();
                    pf.offset = new SizeF(2.02f, 2.14f);
                    pf.footprintSize = new SizeF(55, 28);
                    pf.pin1Position = new PointF(38.0F, 28.5F);
                    pf.width = 0.5f;

                    PointF[] dd = new PointF[] { new PointF(0, 0), new PointF(50.96f, 0.0F), new PointF(0F, 23.71f), new PointF(50.96f, 23.71f) };

                    pf.DrillHoles.AddRange(dd);
                    DrawFootPrint(g, pf);
                }

                {
                    // Voltage Buck Converter
                    FootPrint pf = new FootPrint();
                    pf.offset = new SizeF(2.52f, 2.4f);
                    pf.footprintSize = new SizeF(66, 36);
                    pf.pin1Position = new PointF(40.0F, 65.0F);
                    pf.width = 0.5f;

                    PointF[] dd = new PointF[] { new PointF(0, 0), new PointF(MmFromInch(2.4f), 0F), new PointF(0F, 31.2f), new PointF(MmFromInch(2.4f), 31.2f) };

                    pf.DrillHoles.AddRange(dd);
                    DrawFootPrint(g, pf);
                }

                {
                    // calibration
                    FootPrint pf = new FootPrint();
                    pf.offset = new SizeF(0, 0);
                    pf.footprintSize = new SizeF(170, 170);
                    pf.pin1Position = new PointF(10.0F, 10.0F);
                    pf.width = 0.125f;

                    PointF[] dd = new PointF[] { new PointF(0, 0), new PointF(0, 170F), new PointF(170f, 170f), new PointF(170f, 0f) };

                    pf.DrillHoles.AddRange(dd);
                    DrawFootPrint(g, pf);
                }
            }

            private static float MmFromInch(float inch)
            {
                return inch * 25.4f;
            }

            void DrawFootPrint(Graphics g, FootPrint fp)
            {
                SizeF sp = new SizeF(fp.pin1Position);

                foreach (var p in fp.DrillHoles)
                {
                    DrawLineDot(g, p + sp);
                }            

                SizeF s1 = sp - fp.offset;

                DrawLine(g, fp.width, new PointF(0, 0) + s1, new PointF(0, fp.footprintSize.Height) + s1);
                DrawLine(g, fp.width, new PointF(0, fp.footprintSize.Height) + s1, new PointF(fp.footprintSize.Width, fp.footprintSize.Height) + s1);
                DrawLine(g, fp.width, new PointF(fp.footprintSize.Width, fp.footprintSize.Height) + s1, new PointF(fp.footprintSize.Width, 0) + s1);
                DrawLine(g, fp.width, new PointF(fp.footprintSize.Width, 0) + s1, new PointF(0, 0) + s1);
            }

            public void DrawLineDot(Graphics g, PointF p)
            {
                g.FillRectangle(Brushes.Black, (p.X - 0.25f) * xfactor, (p.Y - 0.25f) * yfactor, 0.5f, 0.5f);
            }

            public void DrawLine(Graphics g, float width, PointF start, PointF end)
            {
                g.DrawLine(new Pen(Brushes.Black, width), start.X * xfactor, start.Y * yfactor, end.X * xfactor, end.Y * yfactor);
            }
        }
    }
}
