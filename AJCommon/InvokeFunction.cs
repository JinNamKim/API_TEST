using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AJParkLib
{
    namespace AJCommon
    {
        public static class InvokeFunction
        {
            #region Panel
            /// <summary>
            /// 패널의 Visible 속성 설정
            /// </summary>
            /// <param name="pn"></param>
            /// <param name="IsVisible"></param>
            public static void PanelVisible(Panel pn, bool IsVisible)
            {
                if (pn.InvokeRequired)
                {
                    pn.Invoke(new MethodInvoker(delegate
                    {
                        pn.Visible = IsVisible;
                    }));
                }
                else
                {
                    pn.Visible = IsVisible;
                }
            }
            #endregion

            #region Label
            /// <summary>
            /// Label의 Text를 변경한다.
            /// </summary>
            /// <param name="pn"></param>
            /// <param name="txt"></param>
            public static void SetLabelText(Label lbl, string txt)
            {
                if (lbl.InvokeRequired)
                {
                    lbl.Invoke(new MethodInvoker(delegate
                    {
                        lbl.Text = txt;
                    }));
                }
                else
                {
                    lbl.Text = txt;
                }
            }
            #endregion

            #region
            public static void AddTextBoxText(TextBox tb, string txt)
            {
                if (tb.InvokeRequired)
                {
                    tb.Invoke(new MethodInvoker(delegate
                    {
                        tb.Text += txt;
                    }));
                }
                else
                {
                    tb.Text += txt;
                }
            }

            public static void SetTextBoxText(TextBox tb, string txt)
            {
                if (tb.InvokeRequired)
                {
                    tb.Invoke(new MethodInvoker(delegate
                    {
                        tb.Text = txt;
                    }));
                }
                else
                {
                    tb.Text = txt;
                }
            }
            #endregion
            #region ListBox
            public static void ListBoxClear(ListBox lb)
            {
                if (lb.InvokeRequired)
                {
                    lb.Invoke(new MethodInvoker(delegate
                    {
                        lb.Items.Clear();
                    }));
                }
                else
                {
                    lb.Items.Clear();
                }
            }

            public static void ListBoxInsertItem(ListBox lb,string txt)
            {
                if (txt == null)
                    return;
                if (lb.InvokeRequired)
                {
                    lb.Invoke(new MethodInvoker(delegate
                    {
                        if (lb.Items.Count > 100)
                            ListBoxClear(lb);
                        lb.Items.Add(txt);
                    }));
                }
                else
                {
                    if (lb.Items.Count > 100)
                        ListBoxClear(lb);
                    lb.Items.Add(txt);
                }
            }
            #endregion

            #region PictureBox

            public static void SetPictureImage(PictureBox box, string Image)
            {
                if (box.InvokeRequired)
                {
                    box.Invoke(new MethodInvoker(delegate
                    {
                        box.ImageLocation = Image;
                    }));
                }
                else
                {
                    box.ImageLocation = Image;
                }
            }
            #endregion
        }
    }
}