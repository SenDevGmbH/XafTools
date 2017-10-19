;#DropDB TestApplicationEasyTest

#Application TestApplication.Win
#Application TestApplication.Web

*FillForm
 User Name = Admin
*Action Log On

#IfDef TestApplication.Win

*Action About...
 
*Action OK

#EndIf

#IfDef TestApplication.Web
*Action Navigation(My Details)
#EndIf
