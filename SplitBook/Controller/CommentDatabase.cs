using SplitBook.Model;
using SplitBook.Request;
using SplitBook.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Controller
{
    public class CommentDatabase
    {
        Action<List<Comment>> callback;

        public CommentDatabase(Action<List<Comment>> callback)
        {
            this.callback = callback;
        }

        public void getComments(int expenseId)
        {
            GetCommentsRequest request = new GetCommentsRequest(expenseId);
            request.getComments(callback);
        }

        public void addComment(int expenseId, string content)
        {
            CreateCommentRequest request = new CreateCommentRequest(expenseId, content);
            request.postComment(callback);
        }
    }
}
