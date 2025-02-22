using Autofac;
using NAPS2.EtoForms;
using NAPS2.EtoForms.Gtk;
using NAPS2.EtoForms.Ui;
using NAPS2.ImportExport;

namespace NAPS2.Modules;

public class GtkModule : GuiModule
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<GtkScannedImagePrinter>().As<IScannedImagePrinter>();
        builder.RegisterType<GtkDarkModeProvider>().As<IDarkModeProvider>().SingleInstance();

        builder.RegisterType<GtkDesktopForm>().As<DesktopForm>();
        builder.RegisterType<GtkPreviewForm>().As<PreviewForm>();
    }
}
