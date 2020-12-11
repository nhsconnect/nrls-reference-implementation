import { IPointer } from "../../../core/interfaces/IPointer";
import { bindable } from "aurelia-framework";

export class ScraOverview {
  @bindable
  pointers: IPointer[] = [];

  navigateToDocumentsTab() {
    const tab = document.querySelector(
      "[role=tab][aria-controls=recordlocator]"
    ) as HTMLElement;
    tab.click();
  }
}
