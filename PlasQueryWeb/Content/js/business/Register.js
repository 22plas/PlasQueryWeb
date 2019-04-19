var _returncode = "";
var _phone = "";
avr _vcodereturn = false;//是否发送验证码
//获取验证码倒计时
$("#code").click(function () {
    var getcodebtn = document.getElementById("code");
    var valthis = $(getcodebtn).html();
    var phonestr = $("#phone").val();
    if (phonestr == "" || phonestr == null || phonestr == undefined) {
        alert('请输入手机号');
        return;
    }
    if (valthis == "获取验证码" || valthis == "重新获取") {
        var a = 59;
        clearInterval(timer);
        $(getcodebtn).html(a);
        var timer = setInterval(function () {
            if (a > 1) {
                a--;
                $(getcodebtn).html(a);
            } else if (a == 1) {
                clearInterval(timer);
                $(getcodebtn).html("重新获取");
            }
        }, 1000);

        $.ajax({
            type: "get",
            url: comm.action("SetCode", "MemberCenter"),
            data: { phone: phonestr },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.State == "Success") {
                    var rs = data.Result;
                    _returncode = rs.code;
                    _phone = phonestr;
                    _vcodereturn = true;
                    console.log(rs);
                    //setTimeout(function () {
                    //    //location = location;
                    //    window.location.reload();
                    //}, 1000);
                }
                else if (data.State == "Fail") {
                    alert("发送失败");
                }
                else {
                    alert("系统异常");
                }
            }
        });
    }
});
//保存
$("#savebtn").click(function () {
    var thiscode = $("#verificationcode").val();
    var account = $("#account").val();
    var password = $("#password").val();
    var passwordtwo = $("#passwordtwo").val();
    var phonestr = $("#phone").val();
    var returnv = verification();
    if (returnv) {

    }

});
//验证保存数据
function verification() {
    var thiscode = $("#verificationcode").val();
    var account = $("#account").val();
    var password = $("#password").val();
    var passwordtwo = $("#passwordtwo").val();
    var phonestr = $("#phone").val();
    var returnstr = true;
    if (returnstr) {
        if (thiscode == "" || thiscode == null || thiscode == undefined || thiscode == "请输入手机验证码") {
            alert("请输入手机验证码");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (account == "" || account == null || account == undefined || account == "请输入帐号") {
            alert("请输入帐号");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (password == "" || password == null || password == undefined || password == "请输入登录密码") {
            alert("请输入登录密码");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (passwordtwo == "" || passwordtwo == null || passwordtwo == undefined || passwordtwo == "请输入确认密码") {
            alert("请输入确认密码");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (password != passwordtwo) {
            alert("两次输入密码不匹配");
            returnstr = false;
        }
    }
    if (returnstr) {
        if (phonestr == "" || phonestr == null || phonestr == undefined || phonestr == "请输入手机号码") {
            alert("请输入手机号码");
            returnstr = false;
        }
    }
    if (returnstr && _vcodereturn) {
        if (phonestr != _phone) {
            alert("当前保存的手机号跟验证的手机号不匹配");
            returnstr = false;
        }
    }

    return returnstr;
}