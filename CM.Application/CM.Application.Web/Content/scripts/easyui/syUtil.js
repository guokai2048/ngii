/**
 * 包含easyui的扩展和常用的方法
 * 
 * @author 孙宇
 * 
 * @version 20120806
 */

var sy = $.extend({}, sy);/* 定义全局对象，类似于命名空间或包的作用 */

/**
 * 使panel和datagrid在加载时提示
 * 
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 */
$.fn.panel.defaults.loadingMessage = '加载中....';
$.fn.datagrid.defaults.loadMsg = '加载中....';

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 避免验证tip屏幕跑偏
 */
var removeEasyuiTipFunction = function() {
	window.setTimeout(function() {
		$('div.validatebox-tip').remove();
	}, 0);
};
$.fn.panel.defaults.onClose = removeEasyuiTipFunction;
$.fn.window.defaults.onClose = removeEasyuiTipFunction;
$.fn.dialog.defaults.onClose = removeEasyuiTipFunction;

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 通用错误提示
 * 
 * 用于datagrid/treegrid/tree/combogrid/combobox/form加载数据出错时的操作
 */
var easyuiErrorFunction = function(XMLHttpRequest) {
	$.messager.progress('close');
	$.messager.alert('错误', XMLHttpRequest.responseText);
};
$.fn.datagrid.defaults.onLoadError = easyuiErrorFunction;
$.fn.treegrid.defaults.onLoadError = easyuiErrorFunction;
$.fn.tree.defaults.onLoadError = easyuiErrorFunction;
$.fn.combogrid.defaults.onLoadError = easyuiErrorFunction;
$.fn.combobox.defaults.onLoadError = easyuiErrorFunction;
$.fn.form.defaults.onLoadError = easyuiErrorFunction;

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 为datagrid、treegrid增加表头菜单，用于显示或隐藏列，注意：冻结列不在此菜单中
 */
var createGridHeaderContextMenu = function(e, field) {
	e.preventDefault();
	var grid = $(this);/* grid本身 */
	var headerContextMenu = this.headerContextMenu;/* grid上的列头菜单对象 */
	if (!headerContextMenu) {
		var tmenu = $('<div style="width:100px;"></div>').appendTo('body');
		var fields = grid.datagrid('getColumnFields');
		for ( var i = 0; i < fields.length; i++) {
			var fildOption = grid.datagrid('getColumnOption', fields[i]);
			if (!fildOption.hidden) {
				$('<div iconCls="icon-ok" field="' + fields[i] + '"/>').html(fildOption.title).appendTo(tmenu);
			} else {
				$('<div iconCls="icon-empty" field="' + fields[i] + '"/>').html(fildOption.title).appendTo(tmenu);
			}
		}
		headerContextMenu = this.headerContextMenu = tmenu.menu({
			onClick : function(item) {
				var field = $(item.target).attr('field');
				if (item.iconCls == 'icon-ok') {
					grid.datagrid('hideColumn', field);
					$(this).menu('setIcon', {
						target : item.target,
						iconCls : 'icon-empty'
					});
				} else {
					grid.datagrid('showColumn', field);
					$(this).menu('setIcon', {
						target : item.target,
						iconCls : 'icon-ok'
					});
				}
			}
		});
	}
	headerContextMenu.menu('show', {
		left : e.pageX,
		top : e.pageY
	});
};
$.fn.datagrid.defaults.onHeaderContextMenu = createGridHeaderContextMenu;
$.fn.treegrid.defaults.onHeaderContextMenu = createGridHeaderContextMenu;

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 防止panel/window/dialog组件超出浏览器边界
 * @param left
 * @param top
 */
var easyuiPanelOnMove = function(left, top) {
	var l = left;
	var t = top;
	if (l < 1) {
		l = 1;
	}
	if (t < 1) {
		t = 1;
	}
	var width = parseInt($(this).parent().css('width')) + 14;
	var height = parseInt($(this).parent().css('height')) + 14;
	var right = l + width;
	var buttom = t + height;
	var browserWidth = $(window).width();
	var browserHeight = $(window).height();
	if (right > browserWidth) {
		l = browserWidth - width;
	}
	if (buttom > browserHeight) {
		t = browserHeight - height;
	}
	$(this).parent().css({/* 修正面板位置 */
		left : l,
		top : t
	});
};
$.fn.dialog.defaults.onMove = easyuiPanelOnMove;
$.fn.window.defaults.onMove = easyuiPanelOnMove;
$.fn.panel.defaults.onMove = easyuiPanelOnMove;

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * panel关闭时回收内存
 */
$.fn.panel.defaults.onBeforeDestroy = function() {
	var frame = $('iframe', this);
	try {
		if (frame.length > 0) {
			for ( var i = 0; i < frame.length; i++) {
				frame[i].contentWindow.document.write('');
				frame[i].contentWindow.close();
			}
			frame.remove();
			if ($.browser.msie) {
				CollectGarbage();
			}
		}
	} catch (e) {
	}
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 扩展validatebox，添加验证两次密码功能
 */
$.extend($.fn.validatebox.defaults.rules, {
	eqPassword : {
		validator : function(value, param) {
			return value == $(param[0]).val();
		},
		message : '密码不一致！'
	}
});

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 扩展datagrid，添加动态增加或删除Editor的方法
 * 
 * 例子如下，第二个参数可以是数组
 * 
 * datagrid.datagrid('removeEditor', 'cpwd');
 * 
 * datagrid.datagrid('addEditor', [ { field : 'ccreatedatetime', editor : { type : 'datetimebox', options : { editable : false } } }, { field : 'cmodifydatetime', editor : { type : 'datetimebox', options : { editable : false } } } ]);
 * 
 */
$.extend($.fn.datagrid.methods, {
	addEditor : function(jq, param) {
		if (param instanceof Array) {
			$.each(param, function(index, item) {
				var e = $(jq).datagrid('getColumnOption', item.field);
				e.editor = item.editor;
			});
		} else {
			var e = $(jq).datagrid('getColumnOption', param.field);
			e.editor = param.editor;
		}
	},
	removeEditor : function(jq, param) {
		if (param instanceof Array) {
			$.each(param, function(index, item) {
				var e = $(jq).datagrid('getColumnOption', item);
				e.editor = {};
			});
		} else {
			var e = $(jq).datagrid('getColumnOption', param);
			e.editor = {};
		}
	}
});

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 扩展datagrid的editor
 * 
 * 增加带复选框的下拉树
 * 
 * 增加日期时间组件editor
 * 
 * 增加多选combobox组件
 */
$.extend($.fn.datagrid.defaults.editors, {
	combocheckboxtree : {
		init : function(container, options) {
			var editor = $('<input />').appendTo(container);
			options.multiple = true;
			editor.combotree(options);
			return editor;
		},
		destroy : function(target) {
			$(target).combotree('destroy');
		},
		getValue : function(target) {
			return $(target).combotree('getValues').join(',');
		},
		setValue : function(target, value) {
			$(target).combotree('setValues', sy.getList(value));
		},
		resize : function(target, width) {
			$(target).combotree('resize', width);
		}
	},
	datetimebox : {
		init : function(container, options) {
			var editor = $('<input />').appendTo(container);
			editor.datetimebox(options);
			return editor;
		},
		destroy : function(target) {
			$(target).datetimebox('destroy');
		},
		getValue : function(target) {
			return $(target).datetimebox('getValue');
		},
		setValue : function(target, value) {
			$(target).datetimebox('setValue', value);
		},
		resize : function(target, width) {
			$(target).datetimebox('resize', width);
		}
	},
	multiplecombobox : {
		init : function(container, options) {
			var editor = $('<input />').appendTo(container);
			options.multiple = true;
			editor.combobox(options);
			return editor;
		},
		destroy : function(target) {
			$(target).combobox('destroy');
		},
		getValue : function(target) {
			return $(target).combobox('getValues').join(',');
		},
		setValue : function(target, value) {
			$(target).combobox('setValues', sy.getList(value));
		},
		resize : function(target, width) {
			$(target).combobox('resize', width);
		}
	}
});

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * @param options
 */
sy.dialog = function(options) {
	var opts = $.extend({
		modal : true,
		onClose : function() {
			$(this).dialog('destroy');
		}
	}, options);
	return $('<div/>').dialog(opts);
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * @param title
 *            标题
 * 
 * @param msg
 *            提示信息
 * 
 * @param fun
 *            回调方法
 */
sy.messagerConfirm = function(title, msg, fn) {
	return $.messager.confirm(title, msg, fn);
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 */
sy.messagerShow = function(options) {
	return $.messager.show(options);
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 */
sy.messagerAlert = function(title, msg, icon, fn) {
	return $.messager.alert(title, msg, icon, fn);
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI,jQuery cookie plugin
 * 
 * 更换EasyUI主题的方法
 * 
 * @param themeName
 *            主题名称
 */
sy.changeTheme = function(themeName) {
	var $easyuiTheme = $('#easyuiTheme');
	var url = $easyuiTheme.attr('href');
	var href = url.substring(0, url.indexOf('themes')) + 'themes/' + themeName + '/easyui.css';
	$easyuiTheme.attr('href', href);

	var $iframe = $('iframe');
	if ($iframe.length > 0) {
		for ( var i = 0; i < $iframe.length; i++) {
			var ifr = $iframe[i];
			$(ifr).contents().find('#easyuiTheme').attr('href', href);
		}
	}

	$.cookie('easyuiThemeName', themeName, {
		expires : 7
	});
};

/**
 * @author 孙宇
 * 
 * 获得项目根路径
 * 
 * 使用方法：sy.bp();
 * 
 * @returns 项目根路径
 */
sy.bp = function() {
	var curWwwPath = window.document.location.href;
	var pathName = window.document.location.pathname;
	var pos = curWwwPath.indexOf(pathName);
	var localhostPaht = curWwwPath.substring(0, pos);
	var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
	return (localhostPaht + projectName);
};

/**
 * @author 孙宇
 * 
 * 使用方法:sy.pn();
 * 
 * @returns 项目名称(/sshe)
 */
sy.pn = function() {
	return window.document.location.pathname.substring(0, window.document.location.pathname.indexOf('\/', 1));
};

/**
 * @author 孙宇
 * 
 * 增加formatString功能
 * 
 * 使用方法：sy.fs('字符串{0}字符串{1}字符串','第一个变量','第二个变量');
 * 
 * @returns 格式化后的字符串
 */
sy.fs = function(str) {
	for ( var i = 0; i < arguments.length - 1; i++) {
		str = str.replace("{" + i + "}", arguments[i + 1]);
	}
	return str;
};

/**
 * @author 孙宇
 * 
 * 增加命名空间功能
 * 
 * 使用方法：sy.ns('jQuery.bbb.ccc','jQuery.eee.fff');
 */
sy.ns = function() {
	var o = {}, d;
	for ( var i = 0; i < arguments.length; i++) {
		d = arguments[i].split(".");
		o = window[d[0]] = window[d[0]] || {};
		for ( var k = 0; k < d.slice(1).length; k++) {
			o = o[d[k + 1]] = o[d[k + 1]] || {};
		}
	}
	return o;
};

/**
 * @author 郭华(夏悸)
 * 
 * 生成UUID
 * 
 * @returns UUID字符串
 */
sy.random4 = function() {
	return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
};
sy.UUID = function() {
	return (sy.random4() + sy.random4() + "-" + sy.random4() + "-" + sy.random4() + "-" + sy.random4() + "-" + sy.random4() + sy.random4() + sy.random4());
};

/**
 * @author 孙宇
 * 
 * 获得URL参数
 * 
 * @returns 对应名称的值
 */
sy.getUrlParam = function(name) {
	var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
	var r = window.location.search.substr(1).match(reg);
	if (r != null)
		return unescape(r[2]);
	return null;
};

/**
 * @author 孙宇
 * 
 * 接收一个以逗号分割的字符串，返回List，list里每一项都是一个字符串
 * 
 * @returns list
 */
sy.getList = function(value) {
	if (value != undefined && value != '') {
		var values = [];
		var t = value.split(',');
		for ( var i = 0; i < t.length; i++) {
			values.push('' + t[i]);/* 避免他将ID当成数字 */
		}
		return values;
	} else {
		return [];
	}
};

/**
 * @author 孙宇
 * 
 * @requires jQuery
 * 
 * 判断浏览器是否是IE并且版本小于8
 * 
 * @returns true/false
 */
sy.isLessThanIe8 = function() {
	return ($.browser.msie && $.browser.version < 8);
};

/**
 * @author 孙宇
 * 
 * @requires jQuery
 * 
 * 将form表单元素的值序列化成对象
 * 
 * @returns object
 */
sy.serializeObject = function(form) {
	var o = {};
	$.each(form.serializeArray(), function(index) {
		if (o[this['name']]) {
			o[this['name']] = o[this['name']] + "," + this['value'];
		} else {
			o[this['name']] = this['value'];
		}
	});
	return o;
};

/**
 * 
 * 将JSON对象转换成字符串
 * 
 * @param o
 * @returns string
 */
sy.jsonToString = function(o) {
	var r = [];
	if (typeof o == "string")
		return "\"" + o.replace(/([\'\"\\])/g, "\\$1").replace(/(\n)/g, "\\n").replace(/(\r)/g, "\\r").replace(/(\t)/g, "\\t") + "\"";
	if (typeof o == "object") {
		if (!o.sort) {
			for ( var i in o)
				r.push(i + ":" + obj2str(o[i]));
			if (!!document.all && !/^\n?function\s*toString\(\)\s*\{\n?\s*\[native code\]\n?\s*\}\n?\s*$/.test(o.toString)) {
				r.push("toString:" + o.toString.toString());
			}
			r = "{" + r.join() + "}";
		} else {
			for ( var i = 0; i < o.length; i++)
				r.push(obj2str(o[i]));
			r = "[" + r.join() + "]";
		}
		return r;
	}
	return o.toString();
};

/**
 * @author 郭华(夏悸)
 * 
 * 格式化日期时间
 * 
 * @param format
 * @returns
 */
Date.prototype.format = function(format) {
	if (isNaN(this.getMonth())) {
		return '';
	}
	if (!format) {
		format = "yyyy-MM-dd hh:mm:ss";
	}
	var o = {
		/* month */
		"M+" : this.getMonth() + 1,
		/* day */
		"d+" : this.getDate(),
		/* hour */
		"h+" : this.getHours(),
		/* minute */
		"m+" : this.getMinutes(),
		/* second */
		"s+" : this.getSeconds(),
		/* quarter */
		"q+" : Math.floor((this.getMonth() + 3) / 3),
		/* millisecond */
		"S" : this.getMilliseconds()
	};
	if (/(y+)/.test(format)) {
		format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
	}
	for ( var k in o) {
		if (new RegExp("(" + k + ")").test(format)) {
			format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
		}
	}
	return format;
};

/**
 * @author 孙宇
 * 
 * @requires jQuery
 * 
 * 改变jQuery的AJAX默认属性和方法
 */
$.ajaxSetup({
	type : 'POST',
	error : function(XMLHttpRequest, textStatus, errorThrown) {
		$.messager.progress('close');
		$.messager.alert('错误', XMLHttpRequest.responseText);
	}
});

/**
 * @author 夏悸
 * 
 * @requires jQuery,EasyUI
 * 
 * 扩展tree，使其支持平滑数据格式
 */
$.fn.tree.defaults.loadFilter = function(data, parent) {
	var opt = $(this).data().tree.options;
	var idFiled, textFiled, parentField;
	if (opt.parentField) {
		idFiled = opt.idFiled || 'id';
		textFiled = opt.textFiled || 'text';
		parentField = opt.parentField;
		var i, l, treeData = [], tmpMap = [];
		for (i = 0, l = data.length; i < l; i++) {
			tmpMap[data[i][idFiled]] = data[i];
		}
		for (i = 0, l = data.length; i < l; i++) {
			if (tmpMap[data[i][parentField]] && data[i][idFiled] != data[i][parentField]) {
				if (!tmpMap[data[i][parentField]]['children'])
					tmpMap[data[i][parentField]]['children'] = [];
				data[i]['text'] = data[i][textFiled];
				tmpMap[data[i][parentField]]['children'].push(data[i]);
			} else {
				data[i]['text'] = data[i][textFiled];
				treeData.push(data[i]);
			}
		}
		return treeData;
	}
	return data;
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 扩展treegrid，使其支持平滑数据格式
 */
$.fn.treegrid.defaults.loadFilter = function(data, parentId) {
	var opt = $(this).data().treegrid.options;
	var idFiled, textFiled, parentField;
	if (opt.parentField) {
		idFiled = opt.idFiled || 'id';
		textFiled = opt.textFiled || 'text';
		parentField = opt.parentField;
		var i, l, treeData = [], tmpMap = [];
		for (i = 0, l = data.length; i < l; i++) {
			tmpMap[data[i][idFiled]] = data[i];
		}
		for (i = 0, l = data.length; i < l; i++) {
			if (tmpMap[data[i][parentField]] && data[i][idFiled] != data[i][parentField]) {
				if (!tmpMap[data[i][parentField]]['children'])
					tmpMap[data[i][parentField]]['children'] = [];
				data[i]['text'] = data[i][textFiled];
				tmpMap[data[i][parentField]]['children'].push(data[i]);
			} else {
				data[i]['text'] = data[i][textFiled];
				treeData.push(data[i]);
			}
		}
		return treeData;
	}
	return data;
};

/**
 * @author 孙宇
 * 
 * @requires jQuery,EasyUI
 * 
 * 创建一个模式化的dialog
 * 
 * @returns $.modalDialog.handler 这个handler代表弹出的dialog句柄
 * 
 * @returns $.modalDialog.xxx 这个xxx是可以自己定义名称，主要用在弹窗关闭时，刷新某些对象的操作，可以将xxx这个对象预定义好
 */
$.modalDialog = function(options) {
	if ($.modalDialog.handler == undefined) {// 避免重复弹出
		var opts = $.extend({
			title : '',
			width : 840,
			height : 680,
			modal : true,
			onClose : function() {
				$.modalDialog.handler = undefined;
				$(this).dialog('destroy');
			},
			onOpen : function() {
				parent.$.messager.progress({
					title : '提示',
					text : '数据处理中，请稍后....'
				});
			}
		}, options);
		opts.modal = true;// 强制此dialog为模式化，无视传递过来的modal参数
		return $.modalDialog.handler = $('<div/>').dialog(opts);
	}
};

/**
 * grid tooltip参数
 * 
 * @author 孙宇
 */
var gridTooltipOptions = {
	tooltip : function(jq, fields) {
		return jq.each(function() {
			var panel = $(this).datagrid('getPanel');
			if (fields && typeof fields == 'object' && fields.sort) {
				$.each(fields, function() {
					var field = this;
					bindEvent($('.datagrid-body td[field=' + field + '] .datagrid-cell', panel));
				});
			} else {
				bindEvent($(".datagrid-body .datagrid-cell", panel));
			}
		});

		function bindEvent(jqs) {
			jqs.mouseover(function() {
				var content = $(this).text();
				if (content.replace(/(^\s*)|(\s*$)/g, '').length > 5) {
					$(this).tooltip({
						content : content,
						trackMouse : true,
						position : 'bottom',
						onHide : function() {
							$(this).tooltip('destroy');
						},
						onUpdate : function(p) {
							var tip = $(this).tooltip('tip');
							if (parseInt(tip.css('width')) > 500) {
								tip.css('width', 500);
							}
						}
					}).tooltip('show');
				}
			});
		}
	}
};
 
/**
 * 扩展easyui中datagrid的footer
 * $('#gridTable').datagrid('statistics', 'guige');在guige列增加合计，在列中 sum: 'true'
 * @author wangw
 */
$.extend($.fn.datagrid.methods, {
    statistics: function (jq, field) {
        var opt = $(jq).datagrid('options').columns;
        var rows = $(jq).datagrid("getRows");
        var footer = new Array();
        footer['sum'] = "";
        var tempHL = 0;//多行表头时，
        if (opt.length > 1)
            tempHL = opt.length - 1;
        for (var i = 0; i < opt[tempHL].length; i++) {
            if (opt[tempHL][i].sum) {
                footer['sum'] = footer['sum'] + sum(opt[tempHL][i].field) + ',';
            }
        }
        var footerObj = new Array();
        if (footer['sum'] != "") {
            var tmp = '{' + footer['sum'].substring(0, footer['sum'].length - 1) + "}";
            var obj = eval('(' + tmp + ')');
            if (field != undefined) {
                footer['sum'] += '"' + field + '":"<b>合计:</b>"';//第0列显示为合计
                obj = eval('({' + footer['sum'] + '})');
            } else {
                obj[opt[tempHL][0].field] = "<b>合计:</b>" + obj[opt[tempHL][0].field];
            }
            footerObj.push(obj);
        }
        if (footerObj.length > 0) {
            $(jq).datagrid('reloadFooter', footerObj);
        }
        function sum(filed) {
            var sumNum = 0;
            var str = "";
            for (var i = 0; i < rows.length; i++) {
                var num = rows[i][filed];
                if (num != undefined) {
                    sumNum += Number(num);
                }
            }
            return '"' + filed + '":"' + sumNum.toFixed(2) + '"';
        }
    }
});
/**
 * 扩展easyui中datagrid的getRows
 * $('#gridTable').datagrid('getCustomRows');在列中 ignore: 'true'
 * @author wangw
 */
$.extend($.fn.datagrid.methods, {
    getCustomRows: function (jq) {
        var opt = $(jq).datagrid('options').columns;
        var rows = $(jq).datagrid("getRows");

        var fields = new Array();
        for (var i = 0; i < opt[0].length; i++) {
            if (opt[0][i].ignore) {
                fields.push(opt[0][i].field);
            }
        }

        if (fields.length > 0) {
            for (var i = 0; i < rows.length; i++) {
                for (var j = 0; j < fields.length; j++) {
                    var row = rows[i];
                    delete row[fields[j]];
                }
            }
        }
        return rows;
    }
});

/**
 * Datagrid扩展方法tooltip 基于Easyui 1.3.3，可用于Easyui1.3.3+
 * 
 * 简单实现，如需高级功能，可以自由修改
 * 
 * 使用说明:
 * 
 * 在easyui.min.js之后导入本js
 * 
 * 代码案例:
 * 
 * $("#dg").datagrid('tooltip'); 所有列
 * 
 * $("#dg").datagrid('tooltip',['productid','listprice']); 指定列
 * 
 * @author 夏悸
 */
$.extend($.fn.datagrid.methods, gridTooltipOptions);


/**
 * 实现tabs组件IFrame模式的遮罩效果
 * 
 */
$.extend($.fn.tabs.methods, {
	/**
     * tabs组件每个tab panel对应的小工具条绑定的事件没有传递事件参数
     * 本函数修正这个问题
     * @param {[type]} jq [description]
     */
    addEventParam: function(jq) {
        return jq.each(function() {
            var that = this;
            var headers = $(this).find('>div.tabs-header>div.tabs-wrap>ul.tabs>li');
            headers.each(function(i) {
                var tools = $(that).tabs('getTab', i).panel('options').tools;
                if (typeof tools != "string") {
                    $(this).find('>span.tabs-p-tool a').each(function(j) {
                        $(this).unbind('click').bind("click", {
                            handler: tools[j].handler
                        }, function(e) {
                            if ($(this).parents("li").hasClass("tabs-disabled")) {
                                return;
                            }
                            e.data.handler.call(this, e);
                        });
                    });
                }
            })
        });
    },
    /**
     * 加载iframe内容  
     * @param  {jq Object} jq     [description]  
     * @param  {Object} params    params.which:tab的标题或者index;params.iframe:iframe的相关参数  
     * @return {jq Object}        [description]  
     */  
    loadTabIframe:function(jq,params){   
        return jq.each(function(){   
            var $tab = $(this).tabs('getTab',params.which);   
            if($tab==null) return;   
  
            var $tabBody = $tab.panel('body');   
  
            //销毁已有的iframe   
            var $frame=$('iframe', $tabBody);   
            if($frame.length>0){   
                try{//跨域会拒绝访问，这里处理掉该异常   
                    $frame[0].contentWindow.document.write('');   
                    $frame[0].contentWindow.close();   
                }catch(e){   
                    //Do nothing   
                }   
                $frame.remove();   
                if($.browser.msie){   
                    CollectGarbage();   
                }   
            }   
            $tabBody.html('');   
  
            $tabBody.css({'overflow':'hidden','position':'relative'});   
            var $mask = $('<div style="position:absolute;z-index:2;width:100%;height:100%;background:#ccc;z-index:1000;opacity:0.3;filter:alpha(opacity=30);"><div>').appendTo($tabBody);   
            var $maskMessage = $('<div class="mask-message" style="z-index:3;width:auto;height:16px;line-height:16px;position:absolute;top:50%;left:50%;margin-top:-20px;margin-left:-92px;border:2px solid #d4d4d4;padding: 12px 5px 10px 30px;background: #ffffff url(\'../style/images/loading2.gif\') no-repeat scroll 5px center;">' + (params.iframe.message || '页面加载中，请稍后....') + '</div>').appendTo($tabBody);   
            var $containterMask = $('<div style="position:absolute;width:100%;height:100%;z-index:1;background:#fff;"></div>').appendTo($tabBody);   
            var $containter = $('<div style="position:absolute;width:100%;height:100%;z-index:0;"></div>').appendTo($tabBody);   
  
            var iframe = document.createElement("iframe");   
            iframe.src = params.iframe.src;   
            iframe.frameBorder = params.iframe.frameBorder || 0;   
            iframe.height = params.iframe.height || '100%';   
            iframe.width = params.iframe.width || '100%';   
            if (iframe.attachEvent){   
                iframe.attachEvent("onload", function(){   
                    $([$mask[0],$maskMessage[0]]).fadeOut(params.iframe.delay || 'slow',function(){   
                        $(this).remove();   
                        if($(this).hasClass('mask-message')){   
                            $containterMask.fadeOut(params.iframe.delay || 'slow',function(){   
                                $(this).remove();   
                            });   
                        }   
                    });   
                });   
            } else {   
                iframe.onload = function(){   
                    $([$mask[0],$maskMessage[0]]).fadeOut(params.iframe.delay || 'slow',function(){   
                        $(this).remove();   
                        if($(this).hasClass('mask-message')){   
                            $containterMask.fadeOut(params.iframe.delay || 'slow',function(){   
                                $(this).remove();   
                            });   
                        }   
                    });   
                };   
            }   
            $containter[0].appendChild(iframe);   
        });   
    },   
    /**
     * 增加iframe模式的标签页  
     * @param {[type]} jq     [description]  
     * @param {[type]} params [description]  
     */  
    addIframeTab:function(jq,params){   
        return jq.each(function(){   
            if(params.tab.href){   
                delete params.tab.href;   
            }   
            $(this).tabs('add',params.tab);   
            $(this).tabs('loadTabIframe',{'which':params.tab.title,'iframe':params.iframe});   
        });   
    },   
    /**
     * 更新tab的iframe内容  
     * @param  {jq Object} jq     [description]  
     * @param  {Object} params [description]  
     * @return {jq Object}        [description]  
     */  
    updateIframeTab:function(jq,params){   
        return jq.each(function(){   
            params.iframe = params.iframe || {};   
            if(!params.iframe.src){   
                var $tab = $(this).tabs('getTab',params.which);   
                if($tab==null) return;   
                var $tabBody = $tab.panel('body');   
                var $iframe = $tabBody.find('iframe');   
                if($iframe.length===0) return;   
                $.extend(params.iframe,{'src':$iframe.attr('src')});   
            }   
            $(this).tabs('loadTabIframe',params);   
        });   
    }   
});