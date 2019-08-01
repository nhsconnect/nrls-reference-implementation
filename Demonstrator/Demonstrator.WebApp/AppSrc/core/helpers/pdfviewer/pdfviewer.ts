﻿import { bindable, bindingMode, observable, inject, Loader } from 'aurelia-framework';
import { PDFJS } from 'pdfjs-dist';
import { IPointerDocument } from '../../interfaces/IPointerDocument';
import { StringHelper } from '../converters/string';


@inject(Loader)
export class pdfviewer {

    @bindable closeViewer;

    @bindable({ defaultBindingMode: bindingMode.twoWay })
    pdfDoc: IPointerDocument;

    private worker: any;
    private pageRendering: boolean = false;
    private pageNumPending?: number;
    private canvas: HTMLCanvasElement;
    private canvasContext?: CanvasRenderingContext2D;
    private canDestroy: boolean = false;
    private viewerContainer: JQuery<HTMLElement>;
    private pdfContainer: JQuery<HTMLElement>;

    constructor(private loader) {

    }

    bind() {

        PDFJS.GlobalWorkerOptions.workerSrc = this.loader.normalizeSync('pdfjs-dist/build/pdf.worker.js');

        this.worker = new PDFJS.PDFWorker();
    }

    attached() {

        this.viewerContainer = $(".pdf-viewer-container");
        this.pdfContainer = $("#pdfCanvasContainer");
        this.canvas = document.getElementById("pdfCanvas") as HTMLCanvasElement;
        this.canvasContext = this.canvas.getContext('2d') || undefined;
    }

    detached() {

        if (this.canDestroy) {

            if (this.pdfDoc.document) {
                this.pdfDoc.document.destroy();
            }

            if (this.worker) {
                this.worker.destroy();
            }

            if (this.canvasContext) {
                this.canvasContext.clearRect(0, 0, this.canvas.width, this.canvas.height);
            }
        }
    }

    loadDocument() {
        var pdfAsArray = StringHelper.convertDataURIToBinary(this.pdfDoc.url);

        var pdfDocument = PDFJS.getDocument({ data: pdfAsArray, worker: this.worker });

        pdfDocument.promise.then((pdf) => {

                this.pdfDoc.document = pdf;

                this.renderPage(this.pdfDoc.currentPage);
            
            }, (reason) => {
                // PDF loading error
                console.error("PDF loading error", reason);
        });
    }

    renderPage(pageNumber: number) {

        this.pageRendering = true;

        // Fetch the first page
        this.pdfDoc.document.getPage(pageNumber).then((page) => {

            let viewport = page.getViewport(this.pdfDoc.scale);

            this.pdfDoc.scale = (this.viewerContainer.height() || 0) / (viewport.height + 40);

            //re-scale
            viewport = page.getViewport(this.pdfDoc.scale);

            // Prepare canvas using PDF page dimensions

            if (this.canvas && this.canvasContext) {

                this.canvas.height = viewport.height;
                this.canvas.width = viewport.width;

                this.pdfContainer.height(viewport.height);
                this.pdfContainer.width(viewport.width);

                // Render PDF page into canvas context
                let renderContext = {
                    canvasContext: this.canvasContext,
                    viewport: viewport
                };

                let renderTask = page.render(renderContext);

                renderTask.promise.then(() => {

                    this.pageRendering = false;

                    if (this.pageNumPending !== undefined) {
                        // New page rendering is pending
                        this.renderPage(this.pageNumPending);
                        this.pageNumPending = undefined;
                    } else {
                        this.canDestroy = true;
                    }

                });
            }
        });
    }

    queueRenderPage(pageNum: number) {

        if (this.pageRendering) {
            this.pageNumPending = pageNum;
        } else {
            this.renderPage(pageNum);
        }
    }

    renderPrevPage() {
        if (this.pdfDoc.currentPage <= 1) {
            return;
        }
        this.pdfDoc.currentPage--;
        this.queueRenderPage(this.pdfDoc.currentPage);
    }

    renderNextPage() {
        if (this.pdfDoc.currentPage >= this.pdfDoc.totalPages) {
            return;
        }
        this.pdfDoc.currentPage++;
        this.queueRenderPage(this.pdfDoc.currentPage);
    }

 }