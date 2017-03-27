using SplitBook.Model;
using SplitBook.Request;
using SplitBook.Utilities;
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

        public async Task GetComments(int expenseId)
        {
            GetCommentsRequest request = new GetCommentsRequest(expenseId);
            await request.GetComments(callback);
        }

        public async Task AddComment(int expenseId, string content)
        {
            CreateCommentRequest request = new CreateCommentRequest(expenseId, content);
            await request.PostComment(callback);
        }
    }
}
