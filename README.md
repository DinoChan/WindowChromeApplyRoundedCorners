![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030163721520-1200218294.png)

## 1. Windows 11 的圆角
在直角统治了微软的 UI 设计多年以后，微软突然把直角骂了一顿，说还是圆角好看，于是 Windows 11 随处都可看到圆角设计。Windows 11 使用 3 个级别的圆角，具体取决于要应用圆角的 UI 组件及该组件相对于相邻元素的排列方式。

|圆角半径     | 使用情况 |
| ----------- | ----------- |
| 8px    | 窗体、Flyout 、弹出菜单等 。另外，当窗体最大化或使用对齐布局时不应用圆角。      |
| 4px   | 页面内的元素，如按钮或列表等。        |
| 0px | 与其它直边相交的直边不使用圆角。 |


也就是说在 Windows 11 上窗体需要应用半径为 8px 的圆角。



## 2. 处理 WindowChrome 的圆角

对于 WPF，如果使用原生 Window 的话不需要额外处理圆角，如果使用了 WindowChrome 自定义窗体样式的话呢？

结论是，如果自定义的 Window 使用了 1 像素的窄边框或无边框的样式，那就可能不需要额外处理。

下面这两张图是同一个自定义的 Window 分别在 Windows 11 和 10 上的样子：




![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030163154236-383647186.png)





可以看到这是个模仿 Windows 10 的 Window 样式，边框只有 1 像素。在 Windows 11 里 WindowChrome 会自动裁剪最外层那 1 像素边框和圆角的其它部分，然后补上一条灰色的边框。这做法简单粗暴但有效。被裁剪过后自定义的 Window 成了一个无边框圆角窗口，看着还挺时髦的。

但这个简单裁剪也可能遇到问题，如果 Window 里的内容正好有个直角的元素，而且这个直角还靠着圆角，就可能被裁剪掉；或者自定义的 Window 使用了无边框的样式，那么这个贴边的边框就会被裁剪掉一像素：

![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030163910718-1156011458.png)


所以 Window 可能不需要额外处理，但内容可能需要，这取决于以前的设计。

还有一种情况，如果这个 Window 的边框大于一个像素（像 Windows 8 那样的粗边框），那就需要修改 Window 样式了：

![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030163950650-383252759.png)






## 3. 我就是喜欢直的，不想要圆角，怎么办

![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030163411563-727387304.png)


上图是 Aero2 的主题样式，这是 Windows 8 以后 WPF 程序的默认主题，再之后微软就没有更新过 WPF 的主题。即使在 Windows 11 上，WPF 的主题也没有获得更新。所以，假使现有的 WPF 程序使用了默认主题，或者自定义的主题按照微软一向的审美全使用了直角元素，那到了 Windows 11 上就会显得格格不入。

微软还是很贴心的，如果我们不想更改样式，可以使用 [DwmSetWindowAttribute](https://docs.microsoft.com/zh-cn/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute) 和 DWM_WINDOW_CORNER_PREFERENCE 控制 Window 的圆角。

``` CS
using System.Runtime.InteropServices;
using System.Windows.Interop;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
        var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

        // ...
        // Perform any other work necessary
        // ...
    }


    // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }

    // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
    // what value of the enum to set.
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern long DwmSetWindowAttribute(IntPtr hwnd,
                                                     DWMWINDOWATTRIBUTE attribute,
                                                     ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                     uint cbAttribute);

    // ...
    // Various other definitions
    // ...
}
```

其中 DWM_WINDOW_CORNER_PREFERENCE 枚举值的含义如下：

| 枚举值	 | 说明 |
| ----------- | ----------- |
|DWMWCP_DEFAULT	| 让系统决定是否对窗口采用圆角设置。|
|DWMWCP_DONOTROUND |	绝不对窗口采用圆角设置。|
|DWMWCP_ROUND |	适当时采用圆角设置。|
| DWMWCP_ROUNDSMALL |	适当时可采用半径较小的圆角设置。|

在 Windows 11 上，使用了上面 4 钟枚举值的窗口效果如下：


![](https://img2020.cnblogs.com/blog/38937/202110/38937-20211030164036057-1553067821.png)





## 4. 参考


[Window(窗体)的UI元素及行为](https://www.cnblogs.com/dino623/p/uielements_of_window.html)：这篇文章主要讨论标准 Window 的 UI 元素和行为。

[使用WindowChrome自定义Window Style](https://www.cnblogs.com/dino623/p/custom_window_style_using_WindowChrome.html)：介绍使用 WindowChrome 自定义 Window 的原理及各种细节。

[使用WindowChrome的问题](https://www.cnblogs.com/dino623/p/problems_of_WindowChrome.html)：介绍如何处理使用 WindowChrome 自定义 Window 会遇到的各种问题。

[WPF 制作高性能的透明背景异形窗口（使用 WindowChrome 而不要使用 AllowsTransparency=True）](https://blog.walterlv.com/post/wpf-transparent-window-without-allows-transparency.html)

[在 Windows 11 的桌面应用中应用圆角](https://docs.microsoft.com/zh-cn/windows/apps/desktop/modernize/apply-rounded-corners)

[在 Windows 11 上，为增强应用功能而可以执行的最常见的 11 种操作](https://docs.microsoft.com/zh-cn/windows/apps/get-started/make-apps-great-for-windows#4-use-the-latest-common-controls)

[Windows 11 中的几何图形](https://docs.microsoft.com/en-us/windows/apps/design/signature-experiences/geometry)





