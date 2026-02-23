-- Fix ChatMessage column names
ALTER TABLE kernelmind.chat_messages RENAME COLUMN session_id TO "SessionId";
ALTER TABLE kernelmind.chat_messages RENAME COLUMN created_at TO "CreatedAt";
