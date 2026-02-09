-- Rename columns to PascalCase to match EF Core conventions
ALTER TABLE kernelmind.pizzas RENAME COLUMN name TO "Name";
ALTER TABLE kernelmind.pizzas RENAME COLUMN description TO "Description";
ALTER TABLE kernelmind.pizzas RENAME COLUMN price TO "Price";
ALTER TABLE kernelmind.pizzas RENAME COLUMN category TO "Category";
ALTER TABLE kernelmind.pizzas RENAME COLUMN ingredients TO "Ingredients";
ALTER TABLE kernelmind.pizzas RENAME COLUMN is_available TO "IsAvailable";
ALTER TABLE kernelmind.pizzas RENAME COLUMN embedding TO "Embedding";
ALTER TABLE kernelmind.pizzas RENAME COLUMN created_at TO "CreatedAt";
ALTER TABLE kernelmind.pizzas RENAME COLUMN updated_at TO "UpdatedAt";

ALTER TABLE kernelmind.customers RENAME COLUMN name TO "Name";
ALTER TABLE kernelmind.customers RENAME COLUMN phone TO "Phone";
ALTER TABLE kernelmind.customers RENAME COLUMN email TO "Email";
ALTER TABLE kernelmind.customers RENAME COLUMN address TO "Address";
ALTER TABLE kernelmind.customers RENAME COLUMN created_at TO "CreatedAt";
ALTER TABLE kernelmind.customers RENAME COLUMN updated_at TO "UpdatedAt";

ALTER TABLE kernelmind.orders RENAME COLUMN status TO "Status";
ALTER TABLE kernelmind.orders RENAME COLUMN total_amount TO "TotalAmount";
ALTER TABLE kernelmind.orders RENAME COLUMN delivery_address TO "DeliveryAddress";
ALTER TABLE kernelmind.orders RENAME COLUMN notes TO "Notes";
ALTER TABLE kernelmind.orders RENAME COLUMN created_at TO "CreatedAt";
ALTER TABLE kernelmind.orders RENAME COLUMN updated_at TO "UpdatedAt";

ALTER TABLE kernelmind.order_items RENAME COLUMN quantity TO "Quantity";
ALTER TABLE kernelmind.order_items RENAME COLUMN unit_price TO "UnitPrice";
ALTER TABLE kernelmind.order_items RENAME COLUMN created_at TO "CreatedAt";

ALTER TABLE kernelmind.chat_sessions RENAME COLUMN session_token TO "SessionToken";
ALTER TABLE kernelmind.chat_sessions RENAME COLUMN context TO "Context";
ALTER TABLE kernelmind.chat_sessions RENAME COLUMN is_active TO "IsActive";
ALTER TABLE kernelmind.chat_sessions RENAME COLUMN last_activity_at TO "LastActivityAt";

ALTER TABLE kernelmind.chat_messages RENAME COLUMN role TO "Role";
ALTER TABLE kernelmind.chat_messages RENAME COLUMN content TO "Content";
ALTER TABLE kernelmind.chat_messages RENAME COLUMN metadata TO "Metadata";

ALTER TABLE kernelmind.vector_documents RENAME COLUMN document_type TO "DocumentType";
ALTER TABLE kernelmind.vector_documents RENAME COLUMN title TO "Title";
