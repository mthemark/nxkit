var ___init_nxkit_xforms_layout___ = function ($, ko, NXKit) {


    return NXKit;
};

if (typeof define === "function" && define.amd) {
    define("nxkit-xforms-layout", ['jquery', 'knockout', 'nxkit'], function ($, ko, NXKit) {
        return ___init_nxkit_xforms_layout___($, ko, NXKit);
    });
} else if (typeof $ === "function" && typeof ko === "object" && typeof NXKit === "object") {
    ___init_nxkit_xforms_layout___($, ko, NXKit);
} else {
    throw new Error("RequireJS missing or jQuery, knockout or NXKit missing.");
}
