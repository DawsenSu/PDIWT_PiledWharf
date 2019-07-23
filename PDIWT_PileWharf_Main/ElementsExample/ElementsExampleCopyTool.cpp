/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExampleCopyTool.cpp $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include "ElementsExample.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;

/*=================================================================================**//**
* Tool class to copy elements between models.
* 1. Key-in the following
*       ELEMENTSEXAMPLE ELEMENT COPY SomeModelName
*       Where SomeModelName should be the name of a model in the current file.
*       If no model is specified, the element is copied inside active model.
* 2. Select element and it will be copied to the supplied model.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
struct ElementsExampleCopyTool : public Bentley::DgnPlatform::DgnElementSetTool
{

private:

WString m_modelName;

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
ElementsExampleCopyTool(int toolId, WCharCP modelName) : Bentley::DgnPlatform::DgnElementSetTool(toolId)
    {
    m_modelName = modelName;
    }

bool CopyElementToModel(ElementRefP elemRef, DgnModelP destinationModel);
void CopyElement(ElementRefP elemRef);

protected:

void _OnPostInstall() override;
bool _OnDataButton(DgnButtonEventCR ev) override;
void _OnRestartTool() override;
void _SetupAndPromptForNextAction () override;

public:

int _OnElementModify(EditElementHandleR element) override;

static void InstallNewInstance (int toolId, WCharCP modelName);

};

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCopyTool::_OnPostInstall()
    {
    //We want to locate elements.
    _SetLocateCursor (true);
    }

/*---------------------------------------------------------------------------------**//**
* Actual Copy element code.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCopyTool::CopyElementToModel(ElementRefP elemRef, DgnModelP destinationModel)
    {
    bool elemCopied = false;
    if(NULL != destinationModel)
        {
        ElementCopyContext copier (destinationModel);
        EditElementHandle copied (elemRef, elemRef->GetDgnModelP());
        if(SUCCESS == copier.DoCopy (copied))
            {
            elemCopied = true;
            WString message = WString(L"Element copied to Model: ") + destinationModel->GetModelName();
            mdlDialog_openInfoBox (message.c_str());
            }
        }

    return elemCopied;
    }

/*---------------------------------------------------------------------------------**//**
* Actual Copy element code.
* This method first verifies that the supplied model does exist.
*   1. If the supplied model exists element is copied to that model.
*   2. If no model name is specified element is copied inside active model.
*   3. If the model does not exist the element is not copied and an error message is displayed.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCopyTool::CopyElement(ElementRefP elemRef)
    {
    if(WString::IsNullOrEmpty(m_modelName.c_str()))
        {
        if(CopyElementToModel(elemRef, ISessionMgr::GetActiveDgnModelP()))
            return;
        }
    else
        {
        DgnFileP activeFile = ISessionMgr::GetActiveDgnFile();
        if(NULL != activeFile)
            {
            ModelId mId = activeFile->FindModelIdByName(m_modelName.c_str());
            if(INVALID_MODELID != mId)
                {
                DgnModelP destinationModel = activeFile->LoadRootModelById (NULL, mId, true, false, false);
                if(CopyElementToModel(elemRef, destinationModel))
                    return;
                }
            }
        }

    mdlDialog_openInfoBox (L"Some problem occured while copying the element. Make sure the model specified does exist.");
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCopyTool::_SetupAndPromptForNextAction ()
    {
    UInt32      msgId;
    msgId = PROMPT_CopyPoint;

    bool doLocate = true;

    if(GetElementAgenda ().GetCount () < 1)
        {
        msgId = PROMPT_SelectElement;
        doLocate = true;
        }
    else
        {
        msgId = PROMPT_CopyPoint;
        doLocate = false;
        }

    mdlAccuSnap_enableSnap (!doLocate);
    __super::_SetLocateCursor (doLocate);

    mdlOutput_rscPrintf (MSG_PROMPT, NULL, STRINGLISTID_Prompts, msgId);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCopyTool::_OnDataButton(DgnButtonEventCR ev)
    {

    //Locate element.
    HitPathCP hitPath = _DoLocate(ev, true, Bentley::DgnPlatform::ComponentMode::Innermost);

    //If an element is located, copy it to the supplied/active model.
    if(NULL != hitPath)
        {
        ElementRefP elemToCopy = hitPath->GetHeadElem();
        CopyElement(elemToCopy);

        //Restart tool for copying more elements.
        _OnRestartTool();
        }

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* Restart tool with same model name.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCopyTool::_OnRestartTool()
    {
    InstallNewInstance(GetToolId (), m_modelName.c_str());
    }

/*---------------------------------------------------------------------------------**//**
* No modification is required so return Error.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
int ElementsExampleCopyTool::_OnElementModify(EditElementHandleR element)
    {
    return ERROR;
    }

/*---------------------------------------------------------------------------------**//**
* Static method to be called from AddIn class.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCopyTool::InstallNewInstance (int toolId, WCharCP modelName)
    {
    ElementsExampleCopyTool* exampleTool = new ElementsExampleCopyTool(toolId, modelName);
    exampleTool->InstallTool();
    }

/*---------------------------------------------------------------------------------**//**
* In this case "unparsed" is expected to be a model name.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCopy (WCharCP unparsed)
    {
    ElementsExampleCopyTool::InstallNewInstance (CMDNAME_ElementsExampleCopy, unparsed);
    }

