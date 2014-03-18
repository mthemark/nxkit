﻿/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/knockout/knockout.d.ts" />
/// <reference path="VisualViewModel.ts" />

module NXKit.Web.XForms {

    export class GroupViewModel extends NXKit.Web.XForms.VisualViewModel {

        constructor(context: KnockoutBindingContext, visual: Visual) {
            super(context, visual);
            var self = this;
        }

    }

}