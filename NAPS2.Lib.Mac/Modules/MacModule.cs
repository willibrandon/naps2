﻿using Autofac;
using NAPS2.EtoForms;
using NAPS2.EtoForms.Mac;
using NAPS2.EtoForms.Ui;
using NAPS2.ImportExport;
using NAPS2.ImportExport.Email;

namespace NAPS2.Modules;

public class MacModule : GuiModule
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<MacScannedImagePrinter>().As<IScannedImagePrinter>();
        builder.RegisterType<AppleMailEmailProvider>().As<IAppleMailEmailProvider>();
        builder.RegisterType<MacDarkModeProvider>().As<IDarkModeProvider>().SingleInstance();
        builder.RegisterType<MacIconProvider>().As<IIconProvider>();

        builder.RegisterType<MacDesktopForm>().As<DesktopForm>();
        builder.RegisterType<MacPreviewForm>().As<PreviewForm>();
    }
}
