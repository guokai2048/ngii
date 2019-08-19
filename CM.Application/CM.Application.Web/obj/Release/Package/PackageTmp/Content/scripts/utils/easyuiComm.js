//easyui datagrid简化
var sxDatagrid = {};
//获取所有正在编辑框中的数据
sxDatagrid.getAllEditorData = function ($dg, rowIndex) {
    var objs = $dg.datagrid("getEditors", rowindex);
    var rowdata = {};
    $.each(objs, function (i, obj) {
        if (obj.type == "textbox")
            rowdata[obj.field] = $(obj.target).textbox("getValue");
        else if (obj.type == "numberbox")
            rowdata[obj.field] = $(obj.target).numberbox("getValue");
        else if (obj.type == "datebox")
            rowdata[obj.field] = $(obj.target).datebox("getValue");
    });
    return rowdata;
}
//插入一行，进行编辑，row:行数据
sxDatagrid.insertEditRow = function ($dg, rowData) {
    var row = $dg.datagrid('getSelected');
    if (row) {
        var index = $dg.datagrid('getRowIndex', row) + 1;
    } else {
        index = 0;
    }
    $dg.datagrid('insertRow', {
        index: index,
        row: rowData || {}
    });
    $dg.datagrid('selectRow', index);
    $dg.datagrid('beginEdit', index);
    return index;
};
////插入一行，进行编辑，row:行数据 新加方法1216
sxDatagrid.insertEditRowV = function ($dg, rowData) {

    var gIndex = $dg.datagrid('getRows').length;
    $dg.datagrid('appendRow', rowData);

    Index = $dg.datagrid('getRows');
    var editIndex = Index.length - 1;
    $dg.datagrid('selectRow', editIndex).datagrid('beginEdit', editIndex);

};
//开启datagrid编辑时 光标落入指定列。 rowindex:行索引，field:字段名
sxDatagrid.focusEditRowCell = function ($dg, rowIndex, field) {
    var ed = $dg.datagrid('getEditor', { index: rowIndex, field: field });//获取当前编辑器
    if (ed) {
        if ($(ed.target).hasClass('textbox-f')) {

            $(ed.target).textbox('textbox').focus();
            setTimeout(function () {
                $(ed.target).textbox('textbox').select();
            }, 0);
        } else {
            $(ed.target).focus();
        }
    }
};
//开启行编辑
sxDatagrid.beginEditRow = function ($dg, rowIndex, focusField) {
    $dg.datagrid("beginEdit", rowIndex);
    sxDatagrid.focusEditRowCell($dg, rowIndex, focusField);
};
//结束行编辑
sxDatagrid.endEditRow = function ($dg) {
    $(".datagrid-row-editing").each(function () {
        $dg.datagrid("endEdit", parseInt($(this).attr("datagrid-row-index")));
    });
};
//取消编辑
sxDatagrid.cancelEditRow = function ($dg, rowIndex) {
    $dg.datagrid('cancelEdit', rowIndex);
};
//删除行
sxDatagrid.deleteRow = function ($dg) {
    //$dg.datagrid("deleteRow",rowIndex);
    var selections = $dg.datagrid('getSelections');
    //var selectRows = [];
    //for (var i = 0; i < selections.length; i++) {
    //    selectRows.push(selections[i]);
    //}
    for (var j = 0; j < selections.length; j++) {
        var index = $dg.datagrid('getRowIndex', selections[j]);
        $dg.datagrid('deleteRow', index);
    }
}
//清空
sxDatagrid.clearData = function ($dg) {
    $dg.datagrid('loadData', { total: 0, rows: [] });
}
//验证所有行数据是否符合格式要求，返回有问题的行号,只对编辑状态的行有效。
sxDatagrid.validata = function ($dg) {
    var rows = $dg.datagrid('getRows'), index, re = "";
    $.each(rows, function (i, v) {
        index = $dg.datagrid("getRowIndex", v);
        if (!$dg.datagrid('validateRow', index)) { re += "," + (i + 1); }
    });
    if (re.length) re = re.substr(1);
    return re;
}
//开启编辑所有行
sxDatagrid.beginEditAllRow = function ($dg) {
    var rows = $dg.datagrid('getRows');
    $.each(rows, function (i, v) {
        index = $dg.datagrid("getRowIndex", v);
        $dg.datagrid('beginEdit', index);
    });
}
