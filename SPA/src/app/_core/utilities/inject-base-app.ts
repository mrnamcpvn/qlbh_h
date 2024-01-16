import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { DestroyService } from "../../_core/services/destroy.service";
import { NgSnotifyService } from '../../_core/services/ng-snotify.service';
import { FunctionUtility } from "../../_core/utilities/function-utility";
import { NgxSpinnerService } from "ngx-spinner";
export abstract class InjectBase {
    protected router = inject(Router);
    protected spinnerService = inject(NgxSpinnerService);
    protected snotifyService = inject(NgSnotifyService);
    protected destroyService = inject(DestroyService);
    protected functionUtility = inject(FunctionUtility);
}
