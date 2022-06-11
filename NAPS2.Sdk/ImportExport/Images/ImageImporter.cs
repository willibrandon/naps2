﻿using System.Threading;
using NAPS2.Scan;

namespace NAPS2.ImportExport.Images;

public class ImageImporter : IImageImporter
{
    private readonly ScanningContext _scanningContext;
    private readonly ImageContext _imageContext;
    private readonly ImportPostProcessor _importPostProcessor;

    public ImageImporter(ScanningContext scanningContext, ImageContext imageContext, ImportPostProcessor importPostProcessor)
    {
        _scanningContext = scanningContext;
        _imageContext = imageContext;
        _importPostProcessor = importPostProcessor;
    }

    public ScannedImageSource Import(string filePath, ImportParams importParams, ProgressHandler progressCallback, CancellationToken cancelToken)
    {
        var sink = new ScannedImageSink();
        Task.Run(() =>
        {
            try
            {
                if (cancelToken.IsCancellationRequested)
                {
                    sink.SetCompleted();
                    return;
                }

                IEnumerable<IMemoryImage> toImport;
                int frameCount;
                try
                {
                    toImport = _imageContext.LoadFrames(filePath, out frameCount);
                }
                catch (Exception e)
                {
                    Log.ErrorException("Error importing image: " + filePath, e);
                    // Handle and notify the user outside the method so that errors importing multiple files can be aggregated
                    throw;
                }

                foreach (var frame in toImport)
                {
                    using (frame)
                    {
                        int i = 0;
                        progressCallback(i++, frameCount);
                        if (cancelToken.IsCancellationRequested)
                        {
                            sink.SetCompleted();
                            return;
                        }

                        bool lossless = frame.OriginalFileFormat is ImageFileFormat.Bmp or ImageFileFormat.Png;
                        var image = _scanningContext.CreateProcessedImage(
                            frame,
                            BitDepth.Color,
                            lossless,
                            -1,
                            Enumerable.Empty<Transform>());
                        image = _importPostProcessor.AddPostProcessingData(
                            image,
                            frame,
                            importParams.ThumbnailSize,
                            importParams.BarcodeDetectionOptions,
                            true);

                        sink.PutImage(image);
                    }

                    progressCallback(frameCount, frameCount);
                }

                sink.SetCompleted();
            }
            catch(Exception e)
            {
                sink.SetError(e);
            }
        });
        return sink.AsSource();
    }
}