unit ArcControlMain;
//==============================================================================
interface
//==============================================================================
uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls;
//==============================================================================
type
  TArcControlForm = class(TForm)
    AppStateEdt: TEdit;
    DobEdt: TEdit;
    FirstNameEdt: TEdit;
    GenderEdt: TEdit;
    Label10: TLabel;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    LastNameEdt: TEdit;
    MiddleNameEdt: TEdit;
    NewBtn: TButton;
    NewPanel: TPanel;
    OpenBtn: TButton;
    OpenPanel: TPanel;
    RefNumEdt: TEdit;
    SourceCodeEdt: TEdit;
    SuffixEdt: TEdit;
    TitleEdt: TEdit;
    procedure NewBtnClick(Sender: TObject);
    procedure OpenBtnClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;
//==============================================================================
var
  ArcControlForm: TArcControlForm;
//==============================================================================
implementation
//==============================================================================
uses Arc_TLB, ComObj, ActiveX;
//==============================================================================
{$R *.DFM}
//==============================================================================
procedure TArcControlForm.NewBtnClick(Sender: TObject);
var
  ArcComObject: IArcServer;
begin
  OleCheck(CoCreateInstance(CLASS_ArcServer,
                            nil,
                            CLSCTX_ALL,
                            IID_IArcServer,
                            ArcComObject));
  ArcComObject.NewCase(SourceCodeEdt.Text, TitleEdt.Text, FirstNameEdt.Text,
   MiddleNameEdt.Text, LastNameEdt.Text, SuffixEdt.Text,
   GenderEdt.Text, AppStateEdt.Text, DobEdt.Text);
end;
//==============================================================================
procedure TArcControlForm.OpenBtnClick(Sender: TObject);
var
  ArcComObject: IArcServer;
begin
  if Length(RefNumEdt.Text) < 7 then
  begin
    MessageDlg('ARC Reference Number must be 7 characters long', mtError,  [mbOk], 0);
    Exit;
  end;
  OleCheck(CoCreateInstance(CLASS_ArcServer,
                            nil,
                            CLSCTX_ALL,
                            IID_IArcServer,
                            ArcComObject));
  ArcComObject.OpenCase(RefNumEdt.Text);
end;
//==============================================================================
//==============================================================================
//==============================================================================
end.
