var overall_flag = false;
var filelist = [];
//媒体选中，取消状态
function SelectFile(id) {
    var class_name = "#content_media_" + id;
    var check_class = "checkbox_" + id;
    if ($("input[class=" + check_class + "]:checked").is(':checked')) {
        $("[class=" + check_class + "]").attr("checked", false);//全选 
        $(class_name).css("background-color", "#FFFFFF");
    }
    else {
        $("[class=" + check_class + "]").attr("checked", true);//全选 
        $(class_name).css("background-color", "#e5e5e5");
    }
    IsChecked(); 
}
//媒体全选与取消
function SelectAll() {
    if ($("input[name='selectAll']:checked").is(':checked')) {
        $("[name=checkboxs]").attr("checked", true);//全选 
        $(".content_media").css("background-color", "#e5e5e5");
    }
    else {
        $("[name=checkboxs]").attr("checked", false);//全选 
        $(".content_media").css("background-color", "#FFFFFF");
    }
    IsChecked();
}

//删除文件button是否禁用
function IsChecked() {
    if ($("input[name='checkboxs']:checked").is(':checked')) {
        $("#delete").removeAttr("disabled");
    }
    else {
        $("#delete").attr({ "disabled": "disabled" });
    }
}
//开启幕布(Add)
function ShowDiv() {
    $(".bg").show();
    $(".close_div").show();
    $(".show").show();
}
//关闭幕布(Add)
function HideDiv() {
    if (!overall_flag) {
        $(".bg").hide()
        $(".close_div").hide();
        $(".show").hide();
        filelist = [];
        $("#path").val('');
        GenerateHtml();
    }
    else {
        alert("文件上传中，请勿操作");
    }
}

//选择上传文件
function ChoiceFile() {

    var result = true;
    var temMediaList = [];
    var temMdiaName = [];
    //获取目录id
    var media_group_id = $('#media_group_id').val();
    //获取file上传的文件
    var mediaList = $("#path")[0].files;

    //将文件保存到全局变量
    for (var i = 0; i < mediaList.length; i++) {
        var flag = true;
        for (var j = 0; j < filelist.length; j++) {
            if (filelist[j].name == mediaList[i].name) {
                flag = false;
            }
        }
        if (flag) {
            temMediaList.push(mediaList[i]);
            temMdiaName.push(mediaList[i].name);
        }
    }
    //去验证当前的媒体是否已存在
    if (temMdiaName.length > 0) {
        $.ajax({
            type: "POST",
            url: "/Media/Verification",
            data: { media_group_id: media_group_id, fileNameList: temMdiaName },
            //返回数据的格式
            datatype: "json",
            async: false,
            success: function (data) {
                result = data["result"];
                var isRepeat = data["isRepeat"];
                var messageInfo = data["messageInfo"];
                if (result) {
                    if (isRepeat) {
                        if (!confirm("当前文件已经在服务端存在,是否需要覆盖")) {
                            for (var i = 0; i < messageInfo.length; i++) {
                                for (var j = 0; j < temMediaList.length; j++) {
                                    if (messageInfo[i] == temMediaList[j].name) {
                                        filelist.push(temMediaList[j]);
                                    }
                                }
                            }
                        }
                        else {
                            for (var i = 0; i < temMdiaName.length; i++) {
                                for (var j = 0; j < temMediaList.length; j++) {
                                    if (temMdiaName[i] == temMediaList[j].name) {
                                        filelist.push(temMediaList[j]);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        for (var i = 0; i < messageInfo.length; i++) {
                            for (var j = 0; j < temMediaList.length; j++) {
                                if (messageInfo[i] == temMediaList[j].name) {
                                    filelist.push(temMediaList[j]);
                                }
                            }
                        }
                    }
                }
                
            },
            //调用出错执行的函数
            error: function (data) {
                result = false;
            }
        });

        if (result) {
            GenerateHtml();
        }
        else {
            alert("连接后端失败");
        }
    }
    
}
//重新生成Table
function GenerateHtml() {
    var html = "";
    html += "<thead><tr>";
    html += "<th> <input id=\"checkAll\" name=\"checkAll\" type=\"checkbox\" onclick=\"SelectAllTr()\" value=\"\" /></th>";
    html += "<th>档案名称</th>";
    html += "<th>上传进度</th>";
    html += "<th>上传结果</th>";
    html += "</tr></thead>";

    if (filelist.length > 0) {
        for (var j = 0; j < filelist.length; j++) {
            var upchkId = "upchk_" + j;
            var filename = filelist[j].name;
            var divId = "bar_" + j;
            var processId = "process_" + j;
            html += "<tr>";
            html += "<td><input id=\"" + upchkId + "\" value=\"" + filename + "\" class=\"upchks\" type=\"checkbox\"></td>";
            html += " <td>" + filename + "</td>";
            html += "<td><div id=\"" + divId + "\" style=\"width:1%; height: 80%; background-color:#4cff00;\"></div></td>"
            html += " <td><span id=\"" + processId + "\">0%</span></td>";
            html += "</tr>";
        }
    }
    $("#tb").html(html);
}

function SelectAllTr() {
    if ($("input[name='checkAll']:checked").is(':checked')) {
        $("[class=upchks]").attr("checked", true);//全选       
    }
    else {
        $("[class=upchks]").attr("checked", false);//全选 
    }
}

function DelTr() {
    text = $("input:checkbox[class=upchks]:checked").map(function (index, elem) {

        return $(elem).val();
    }).get().join(',');
    if (text == "") {
    }
    else {
        if (!overall_flag) {
            var arr = text.split(',');
            for (var i in arr) {
                for (var j = 0; j < filelist.length; j++) {
                    if (filelist[j].name == arr[i]) {
                        filelist.splice(j, 1);
                    }
                }
            }
            if (filelist.length <= 0) {
                $("#path").val('');
            }
            GenerateHtml();
        }
        else {
            alert("文件上传中，请勿操作");
        }
    }
}

//提交文件上传
function SubmitUpload(j) {
    if (filelist.length > 0) {
        $('#file_state').text("文件上传中........");
        overall_flag = true;
        var file = filelist[0];
            var guid = "";
            $.ajax({
                type: "GET",
                url: "/Media/GetGuid",
                //返回数据的格式
                datatype: "json",
                async: false,
                success: function (data) {
                    guid = data["guid"];
                },
                //调用出错执行的函数
                error: function (data) {

                }
            }); 
        if (guid == "") {
            guid = file.lastModified;
        }
        AjaxFile(file, guid, j, 0);
    }
}

function AjaxFile(file, guid, j, i) {
    //需要修改的
    var divId = "bar_" + j;
    var processId = "process_" + j;
    //获取api的地址
    var apiUrl = $('#url_upload').val();
    var media_group_id = $('#media_group_id').val();
    //获取文件信息
    var name = file.name;
    var size = file.size;
    var shardSize = 2 * 1024 * 1024;
    var shardCount = Math.ceil(size / shardSize);
    if (i >= shardCount) {
        return;
    }
    //计算每一片的起始与结束位置
    var start = i * shardSize;
    var end = Math.min(size, start + shardSize);
    //构造一个表单，FormData是HTML5新增的
    var form = new FormData();
    form.append("mediaGroupId", media_group_id)
    form.append("data", file.slice(start, end)); //slice方法用于切出文件的一部分
    form.append("guid", guid);
    form.append("mediaName", name);
    form.append("total", shardCount); //总片数
    form.append("index", i + 1); //当前是第几片
    $.ajax({
        url: apiUrl,
        type: "POST",
        data: form,
        async: true,
        dataType:"json",
        processData: false, //很重要，告诉jquery不要对form进行处理
        contentType: false, //很重要，指定为false才能形成正确的Content-Type
        success: function (result) {
            if (result != null) {
                i = result.number++;
                var num = Math.ceil(i * 100 / shardCount);
                var percentage = num + "%";
                $('#' + divId).width(percentage);
                $('#' + processId).text(percentage);
                
                if (result.mergeResult) {
                    $('#' + processId).text("Success");
                    filelist.splice(0, 1);
                    if (filelist.length > 0) {
                        j++;
                        SubmitUpload(j);
                    }
                    else {
                        $('#file_state').text("上传完成");
                        overall_flag = false;
                    }
                }
                else {
                    if (result.errorInfo != "") {
                        $('#' + processId).text(result.errorInfo);
                        filelist.splice(0, 1);
                        if (filelist.length > 0) {
                            j++;
                            SubmitUpload(j);
                        }
                        else {
                            $('#file_state').text("上传完成");
                            overall_flag = false;
                        }
                    }
                }
                AjaxFile(file, guid, j, i);
            }
        },
        error: function (result) {
            var jsonStr = JSON.stringify(result);
            alert(jsonStr);
        }

    });
}
function submitDel() {
    if (confirm("确认删除么?")) {
        var media_group_id = 0;
        if ($("#media_group_id").val() != "") {
            media_group_id = $("#media_group_id").val();
        }
        var id_list = [];
        $('input[name="checkboxs"]:checked').each(function () {//遍历每一个名字为nodes的复选框，其中选中的执行函数    
            id_list.push($(this).val());//将选中的值添加到数组chk_value中    
        });

        if (id_list.length > 0) {
            $.ajax({
                url: '/Media/GetAjaxDel',
                data: { id_list: id_list, media_group_id: media_group_id },
                type: 'post',
                success: function (data) {
                    $('#content_medias').html(data);
                    alert("删除成功");
                },
                error: function (data) {
                    alert("删除失败");
                }
            });
        }
    }
}

function Serch(value) {
    if (value == "") {
        $(".content_media").show();
    }
    else {
        $(".content_media").hide();
        $(".content_media").each(function () {
            if ($(this).html().indexOf(value) > 0) {
                $(this).show();
            }
        });
    }

}


