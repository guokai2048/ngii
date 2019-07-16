$(function () {
    $.getclientdata();

    var depBo = new Object();
    depBo.id = '2d334151-788b-44f9-aa37-2b43bfa52005';
    depBo.name = '箔剪操作';
    userSelf.depBo = depBo;


    var depZha = new Object();
    depZha.id = '1409675f-4335-437d-855d-2b99015bfac4';
    depZha.name = '轧机操作';
    userSelf.depZha = depZha;


    var depFen = new Object();
    depFen.id = '2ea13a6e-6e95-46c3-a1fb-edd5d51ce390';
    depFen.name = '分切操作';
    userSelf.depFen = depFen;

  
     

})
var clientdataItem = [];//数据字典
var clientorganizeData = [];//公司
var clientdepartmentData = []; //部门
var clientpostData = []; //岗位
var clientroleData = [];//角色
var clientuserGroup = [];//用户组
var clientuserData = [];//用户
var authorizeMenuData = [];//菜单
var authorizeButtonData = [];//按钮
var authorizeColumnData = [];//显示列

var sysWorkcenter = [];//工序
var sysWorkcenterJson;
var userSelf = new Object();//存货分类

$.getclientdata = function () {
    $.ajax({
        url: contentPath + "/ClientData/GetClientDataJson",
        type: "post",
        dataType: "json",
        async: false,
        success: function (data) {
            clientdataItem = data.dataItem;
            clientorganizeData = data.organize;
            clientdepartmentData = data.department;
            sysWorkcenter = data.sysWorkcenter;
            sysWorkcenterJson = data.sysWorkcenterJson;
            //clientpostData = data.post;
            clientroleData = data.role;
            //clientuserGroup = data.userGroup;
            //clientuserData = data.user;
            authorizeMenuData = data.authorizeMenu;
            authorizeButtonData = data.authorizeButton;
            //authorizeColumnData = data.authorizeColumn;
            //clientModuleData = data.menuData;
            //clientModuleButtonData = data.buttonData;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            dialogMsg(errorThrown, -1);
        }
    });
}