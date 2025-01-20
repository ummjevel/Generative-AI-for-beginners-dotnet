using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using System.Net;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Reflection.Metadata;
using System.Windows.Media;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0050, CS8600, CS8604

namespace AgentsSK_20_CreativeWriter;

class AgenticWorkflowModel : INotifyPropertyChanged
{
    testWindow02? mainWindow;

    private int _CharacterLimit = 2000;
    public IAgentGroupChat? ChatWorkflow { get; private set; }

    public int CharacterLimit
    {
        get { return _CharacterLimit; }
        set
        {
            if (_CharacterLimit != value)
            {
                _CharacterLimit = value;
                OnPropertyChanged("CharacterLimit");
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _Question = "Get info about Canadian popular stories for kids, and create a kids' story for programmers to learn more about this. the story should be short, 2 paragraphs";
    public string Question
    {
        get { return _Question; }
        set
        {
            if (_Question != value)
            {
                _Question = value;
                OnPropertyChanged("Question");
            }
        }
    }

    public AgenticWorkflowModel(testWindow02 mainWindow)
    {
        this.mainWindow = mainWindow;
        ChatWorkflow = CreativeWriterChatWorkflowFactory.CreateChat(CharacterLimit);        
    }

    public async Task askQuestion()
    {
        if (ChatWorkflow == null)
        {
            updateResponseBox("Error", "No chat workflow is initialized for the selected workflow.");
            return;
        }

        string input = Question;
        ChatWorkflow.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));

        updateResponseBox("Question", input);

        string finalAnswer = "";

        await foreach (var content in ChatWorkflow.InvokeAsync())
        {
            Color color;
            switch (content.AuthorName)
            {
                case "Researcher":
                    color = Colors.DarkOrange;
                    break;
                case "CreativeWriter":
                    color = Colors.DarkGreen;
                    break;
                case "Critic":
                    color = Colors.DarkRed;
                    break;
                default:
                    color = Colors.DarkSlateBlue;
                    break;
            }
            updateResponseBox(content.AuthorName, content.Content, color);
        }
    }

    public void updateResponseBox(string sender, string response)
    {
        updateResponseBox(sender, response, Colors.Black);
    }

    public void updateResponseBox(string sender, string response, Color color)
    {
        //Update mainWindow.ResponseBox to add the sender in bold, a colon, a space, and the response in normal text
        Paragraph paragraph = new Paragraph();
        Bold bold = new Bold(new Run(sender + ": "));
        
        bold.Foreground = new SolidColorBrush(color);
        
        paragraph.Inlines.Add(bold);
        Run run = new Run(response);
        paragraph.Inlines.Add(run);
        mainWindow.ResponseBox.Document.Blocks.Add(paragraph);

        // Scroll to the end after adding new content
        ScrollToEnd(mainWindow.ResponseBox);
    }

    private void ScrollToEnd(RichTextBox richTextBox)
    {
        if (richTextBox != null)
        {
            // Use Dispatcher to ensure it's invoked on the UI thread
            richTextBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                richTextBox.ScrollToEnd();
            }));
        }
    }
}
