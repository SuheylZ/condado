unit Arc_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : $Revision:   1.88.1.0.1.0  $
// File generated on 9/23/2013 3:42:26 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: D:\Projects\ARC\Source\ARC-COM\arc.tlb (1)
// IID\LCID: {A4D01F29-7DC2-4B80-92FE-21FFB063C743}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  ArcMajorVersion = 1;
  ArcMinorVersion = 0;

  LIBID_Arc: TGUID = '{A4D01F29-7DC2-4B80-92FE-21FFB063C743}';

  IID_IArcServer: TGUID = '{7EA7D4AC-17FF-4FFA-8A68-822F3CC8BAEE}';
  CLASS_ArcServer: TGUID = '{D8518139-EFBD-4AE0-9EBC-A37F22F1760A}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IArcServer = interface;
  IArcServerDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ArcServer = IArcServer;


// *********************************************************************//
// Interface: IArcServer
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {7EA7D4AC-17FF-4FFA-8A68-822F3CC8BAEE}
// *********************************************************************//
  IArcServer = interface(IDispatch)
    ['{7EA7D4AC-17FF-4FFA-8A68-822F3CC8BAEE}']
    procedure OpenCase(const RefNum: WideString); safecall;
    procedure NewCase(const SourceCode: WideString; const Title: WideString; 
                      const FirstName: WideString; const MiddleName: WideString; 
                      const LastName: WideString; const Suffix: WideString; 
                      const Gender: WideString; const AppState: WideString; 
                      const BirthDate: WideString); safecall;
  end;

// *********************************************************************//
// DispIntf:  IArcServerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {7EA7D4AC-17FF-4FFA-8A68-822F3CC8BAEE}
// *********************************************************************//
  IArcServerDisp = dispinterface
    ['{7EA7D4AC-17FF-4FFA-8A68-822F3CC8BAEE}']
    procedure OpenCase(const RefNum: WideString); dispid 1;
    procedure NewCase(const SourceCode: WideString; const Title: WideString; 
                      const FirstName: WideString; const MiddleName: WideString; 
                      const LastName: WideString; const Suffix: WideString; 
                      const Gender: WideString; const AppState: WideString; 
                      const BirthDate: WideString); dispid 2;
  end;

// *********************************************************************//
// The Class CoArcServer provides a Create and CreateRemote method to          
// create instances of the default interface IArcServer exposed by              
// the CoClass ArcServer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoArcServer = class
    class function Create: IArcServer;
    class function CreateRemote(const MachineName: string): IArcServer;
  end;

implementation

uses ComObj;

class function CoArcServer.Create: IArcServer;
begin
  Result := CreateComObject(CLASS_ArcServer) as IArcServer;
end;

class function CoArcServer.CreateRemote(const MachineName: string): IArcServer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ArcServer) as IArcServer;
end;

end.
