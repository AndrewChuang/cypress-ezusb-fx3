// BulkLoopDlg.h : header file
//

#if !defined(AFX_BULKLOOPDLG_H__D3E75ECD_0ADD_4838_B4EE_E39E6FC7B4B8__INCLUDED_)
#define AFX_BULKLOOPDLG_H__D3E75ECD_0ADD_4838_B4EE_E39E6FC7B4B8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "CyAPI.h"

#include <initguid.h>

static const GUID GuidInterfaceList[] =
{
    { 0xa5dcbf10, 0x6530, 0x11d2, { 0x90, 0x1f, 0x00, 0xc0, 0x4f, 0xb9, 0x51, 0xed } },
    { 0x53f56307, 0xb6bf, 0x11d0, { 0x94, 0xf2, 0x00, 0xa0, 0xc9, 0x1e, 0xfb, 0x8b } },
    { 0x4d1e55b2, 0xf16f, 0x11Cf, { 0x88, 0xcb, 0x00, 0x11, 0x11, 0x00, 0x00, 0x30 } },
    { 0xad498944, 0x762f, 0x11d0, { 0x8d, 0xcb, 0x00, 0xc0, 0x4f, 0xc3, 0x35, 0x8c } },
    { 0x219d0508, 0x57a8, 0x4ff5, {0x97, 0xa1, 0xbd, 0x86, 0x58, 0x7c, 0x6c, 0x7e   } },
    {0x86e0d1e0L, 0x8089, 0x11d0, {0x9c, 0xe4, 0x08, 0x00, 0x3e, 0x30, 0x1f, 0x73   } },
};

/////////////////////////////////////////////////////////////////////////////
// CBulkLoopDlg dialog

class CBulkLoopDlg : public CDialog
{
    // Construction
public:
    CBulkLoopDlg(CWnd* pParent = NULL);	// standard constructor
    CCyUSBDevice *USBDevice;

    CCyUSBEndPoint *OutEndpt;
    CCyUSBEndPoint *InEndpt;

    int DeviceIndex;

    CWinThread *XferThread;

    int DataFillMethod;
    bool bLooping;
    bool    bPnP_Arrival;
    bool    bPnP_Removal;
    bool    bPnP_DevNodeChange;

    //bool RegForUSBEvents(void);


    // Dialog Data
    //{{AFX_DATA(CBulkLoopDlg)
    enum { IDD = IDD_BULKLOOP_DIALOG };
    CButton	m_DisableTimeoutChkBox;
    CButton	m_TestBtn;
    CButton	m_RefreshBtn;
    CComboBox	m_DeviceListComBox;
    CButton	m_StartBtn;
    CStatic	m_StatusLabel;
    CComboBox	m_FillPatternComBox;
    CButton	m_StopOnErrorChkBox;
    CEdit	m_SeedValue;
    CStatic	m_FailureCount;
    CStatic	m_SuccessCount;
    CEdit	m_XferSize;
    CComboBox	m_InEndptComBox;
    CComboBox	m_OutEndptComBox;
    int		m_DataValueRadioBtns;
    //}}AFX_DATA

    // ClassWizard generated virtual function overrides
    //{{AFX_VIRTUAL(CBulkLoopDlg)
protected:
    virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
    //}}AFX_VIRTUAL


    // Implementation
protected:
    HICON m_hIcon;

    // Generated message map functions
    //{{AFX_MSG(CBulkLoopDlg)
    virtual BOOL OnInitDialog();
    afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
    afx_msg void OnPaint();
    afx_msg HCURSOR OnQueryDragIcon();
    afx_msg void OnStartBtn();
    afx_msg void OnSelchangeOutCombox();
    afx_msg void OnSelchangeInCombox();
    afx_msg void OnResetBtn();
    afx_msg void OnRefreshBtn();
    afx_msg void OnTestBtn();
    afx_msg void OnSelchangeDeviceListCombox();
    //}}AFX_MSG
    afx_msg BOOL OnDeviceChange(UINT EventType, DWORD_PTR dwData);

    DECLARE_MESSAGE_MAP()

private:
    HDEVNOTIFY NotificationHandle;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_BULKLOOPDLG_H__D3E75ECD_0ADD_4838_B4EE_E39E6FC7B4B8__INCLUDED_)
